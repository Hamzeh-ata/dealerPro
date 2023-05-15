namespace test.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using OpenQA.Selenium.Chrome;
    using test.Models;
    using FireSharp;
    using FireSharp.Interfaces;
    using FireSharp.Config;
    using FireSharp.Response;
    using Telerik.JustMock;
    using System.Collections.ObjectModel;
    using OpenQA.Selenium.Interactions;
    using test.Interfaces;

    public class cityCenterLaptopsHub : Hub, ILaptopNamesHub
    {
        private HashSet<string> storedproducts = new HashSet<string>();
        List<string> lapTopSpecifications = new List<string>();
        string endPoint = "Products/Chairs/";
        string storeName = "CityCenter";
        public async Task GetLaptops()
        {
            var chromeOptions = new ChromeOptions();
            var service = ChromeDriverService.CreateDefaultService(@"C:/Users/tsmra/Desktop/chromedriver.exe");
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("window-size=1920,1080");
            using (var driver = new ChromeDriver(service, chromeOptions))
            {
                fireBaseServices _fireBaseServices = new fireBaseServices();
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
                driver.Navigate().GoToUrl("https://citycenter.jo/pc-and-laptops/pc-and-laptops-laptops/gaming-laptop");
                Thread.Sleep(10000);
                Actions actions = new Actions(driver);
                try
                {
                    IWebElement stockStatusForm = wait.Until(driver => driver.FindElement(By.CssSelector(".bf-form")));
                    IWebElement stockStatusHeader = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".bf-attr-block >.bf-attr-header.bf-clickable")));
                    actions.MoveToElement(stockStatusHeader).Click().Build().Perform();
                    Thread.Sleep(10000);
                    IWebElement inStockCheckbox = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#bf-attr-s0_7_60")));
                    if (!inStockCheckbox.Selected)
                    {
                        inStockCheckbox.Click();
                        Thread.Sleep(10000);
                    }
                }
                catch (NoSuchElementException ex)
                {
                    // Handle the exception - e.g. log the error or show an error message
                    Console.WriteLine("Element not found: " + ex.Message);
                }
                catch (WebDriverException ex)
                {
                    // Handle the exception - e.g. log the error or show an error message
                    Console.WriteLine("WebDriver exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle other exceptions - e.g. log the error or show an error message
                    Console.WriteLine("Exception: " + ex.Message);
                }
                while (true)
                {
                    try
                    {
                        Thread.Sleep(10000);
                       var laptops = driver.FindElements(By.CssSelector(".product-layout > .product-thumb"));
                        //var price = driver.FindElements(By.CssSelector("div.caption > p.price"));
                        for (int i = 0; i < laptops.Count(); i++)
                        {
                            Thread.Sleep(5000);
                            string oldPriceBeforeClick = "0";
                            if (wait.Until(driver => laptops[i].FindElements(By.CssSelector("p.price > span.price-old"))).Count > 0)
                            {
                                var oldPriceParentB = wait.Until(driver => laptops[i].FindElement(By.CssSelector("p.price > span.price-old")));
                                var priceIntB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_integer")));
                                var priceDecimalOldB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_decimal")));
                                oldPriceBeforeClick = priceIntB.Text + "." + priceDecimalOldB.Text;
                            }
                            else
                            {
                                oldPriceBeforeClick = "0";
                            }

                            var priceParentB = wait.Until(driver => laptops[i].FindElement(By.CssSelector(".price")));
                            var priceSpanB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span")).Last(e => e.GetAttribute("class") == "tb_integer"));
                            var priceDecimalB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span.tb_decimal")).Last());
                            string priceBeforeClick = priceSpanB.Text + "." + priceDecimalB.Text;
                            var NameBeforeClick = laptops[i].FindElement(By.CssSelector("div.caption > h4"));

                            if (await _fireBaseServices.checkIfAlreadyExists(endPoint, storeName, NameBeforeClick.Text, oldPriceBeforeClick, priceBeforeClick))
                            {
                                Console.WriteLine("The element and the price are already the same and exists");
                                storedproducts.Add(NameBeforeClick.Text);
                                continue;
                            }
                            else
                            {

                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                js.ExecuteScript("arguments[0].scrollIntoView();", laptops[i]);
                                // Wait for the element to become clickable
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(laptops[i]));
                                actions.MoveToElement(laptops[i]).Click().Build().Perform();

                                Thread.Sleep(5000);
                                //wait until laptop name is visible
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".tb_wt h1")));
                                //get laptop name
                                var laptopName = wait.Until(driver => driver.FindElement(By.CssSelector(".tb_wt h1")));
                                //get laptop image
                                var images = wait.Until(driver => driver.FindElement(By.CssSelector("li.mSSlide.active > img")));
                                //get laptop price

                                string oldPrice = "0";
                                if (wait.Until(driver => driver.FindElements(By.CssSelector(".tb_system_product_price > .price > span.price-old"))).Count > 0)
                                {
                                    var oldPriceParent = wait.Until(driver => driver.FindElement(By.CssSelector(".tb_system_product_price > .price > span.price-old")));
                                    var priceInt = wait.Until(driver => oldPriceParent.FindElement(By.CssSelector(".tb_integer")));
                                    var priceDecimalOld = wait.Until(driver => oldPriceParent.FindElement(By.CssSelector(".tb_decimal")));
                                    oldPrice = priceInt.Text + "." + priceDecimalOld.Text;
                                }
                                else
                                {
                                    oldPrice = "0";
                                }

                                var priceParent = wait.Until(driver => driver.FindElement(By.CssSelector(".price")));
                                var priceSpan = wait.Until(driver => priceParent.FindElements(By.CssSelector("span")).Last(e => e.GetAttribute("class") == "tb_integer"));
                                var priceDecimal = wait.Until(driver => priceParent.FindElements(By.CssSelector("span.tb_decimal")).Last());
                                string newPrice = priceSpan.Text + "." + priceDecimal.Text;

                                //GET BRAND 
                                // Locate the div element by its ID
                                IWebElement productInfoSystem = driver.FindElement(By.Id("ProductInfoSystem_Ho7r8pnm"));

                                // Find all the dd elements within the div
                                IList<IWebElement> ddElements = productInfoSystem.FindElements(By.TagName("dd"));

                                // Get the text of the last dd element
                                string brand = ddElements[ddElements.Count - 1].Text;

                                //get laptop stock status
                                var stockStatus = wait.Until(driver => driver.FindElements(By.CssSelector("span"))
                                                         .Where(e => e.GetAttribute("class").Contains("tb_stock_status"))
                                                         .FirstOrDefault());
                                //click on product specifications
                                var tabsContainer = wait.Until(driver => driver.FindElement(By.CssSelector("div.tb_tabs ul.nav-tabs")));
                                var secondTab = tabsContainer.FindElements(By.CssSelector("li"))[1];
                                secondTab.Click();
                                Thread.Sleep(5000);
                                js.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(By.CssSelector("table.table")));
                                IWebElement table = wait.Until(driver => driver.FindElement(By.CssSelector("table.table")));
                                ReadOnlyCollection<IWebElement> rows = wait.Until(driver => table.FindElements(By.CssSelector("tbody > tr ")));
                                // Loop through the rows and extract the data
                                foreach (IWebElement row in rows)
                                {
                                    // Find all the cells in the row
                                    ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));

                                    // Check if the row contains data (i.e. has cells)
                                    if (cells.Count > 0)
                                    {
                                        string heading = cells[0].Text;
                                        string value = cells[1].Text;

                                        var rowObject = new
                                        {
                                            Label = heading,
                                            Value = value
                                        };
                                        lapTopSpecifications.Add(heading + " : " + value);
                                    }
                                }
                                string productUrl = driver.Url;

                                await _fireBaseServices.insertDataIntoFirebase(endPoint, storeName, laptopName.Text, images.GetAttribute("src"), oldPrice, newPrice, stockStatus.Text, brand, lapTopSpecifications,productUrl);
                                storedproducts.Add(laptopName.Text);
                                driver.Navigate().Back();
                                lapTopSpecifications.Clear();
                                // wait until laptop card is visible
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".product-thumb")));
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div.caption > p.price")));
                                laptops = driver.FindElements(By.CssSelector(".product-layout > .product-thumb"));
                            }
                        }
                        
                    }
                    catch (NoSuchElementException ex)
                    {
                        // Handle the exception - e.g. log the error or show an error message
                        Console.WriteLine("Element not found: " + ex.Message);
                    }
                    catch (WebDriverException ex)
                    {
                        // Handle the exception - e.g. log the error or show an error message
                        Console.WriteLine("WebDriver exception: " + ex.Message);
                    }
                  
                    try
                    {
                        var nextButton = driver.FindElement(By.CssSelector("li.next > a"));
                        if (nextButton != null)
                        {
                            Thread.Sleep(10000);
                            actions.MoveToElement(nextButton).Click().Build().Perform();
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        await _fireBaseServices.checkIfElementIsStillExists(endPoint, storeName, storedproducts);
                        storedproducts.Clear();
                        driver.Quit();
                        Console.WriteLine("Finshed");
                        break;
                        
                    }
                }
            }
        }
    }
}
