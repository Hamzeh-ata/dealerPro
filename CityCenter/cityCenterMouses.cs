using Microsoft.AspNetCore.SignalR;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using test.Interfaces;

namespace test.Hubs
{
    public class cityCenterMouses : Hub, IcityCenterMouses
    {
        private HashSet<string> storedproducts = new HashSet<string>();
        string endPoint = "Products/Mouses/";
        string storeName = "CityCenter";
        public async Task GetMouses()
        {
            var chromeOptions = new ChromeOptions();
            var service = ChromeDriverService.CreateDefaultService(@"C:/Users/tsmra/Desktop/chromedriver.exe");
            List<string> liTexts = new List<string>();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("window-size=1920,1080");
            using (var driver = new ChromeDriver(service, chromeOptions))
            {
                fireBaseServices _fireBaseServices = new fireBaseServices();
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 120));
                driver.Navigate().GoToUrl("https://citycenter.jo/gaming/gaming-peripherals/mouse");
                Thread.Sleep(10000);
                Actions actions = new Actions(driver);
                try
                {
                    IWebElement stockStatusForm = wait.Until(driver => driver.FindElement(By.CssSelector(".bf-form ")));
                    IWebElement stockStatusHeader = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".bf-attr-block >.bf-attr-header.bf-clickable")));
                    actions.MoveToElement(stockStatusHeader).Click().Build().Perform();
                    Thread.Sleep(10000);
                    IWebElement inStockCheckbox = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#bf-attr-s0_7_60")));
                    if (!inStockCheckbox.Selected)
                    {
                        inStockCheckbox.Click();
                        Thread.Sleep(5000);
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
                        Thread.Sleep(5000);
                        var mouse = wait.Until(driver => driver.FindElements(By.CssSelector(".product-layout > .product-thumb")));
                        for (int i = 0; i < mouse.Count(); i++) // loop through all CPUs except the last one
                        {

                            Thread.Sleep(5000);
                            string oldPriceBeforeClick = "0";
                            if (wait.Until(driver => mouse[i].FindElements(By.CssSelector("p.price > span.price-old"))).Count > 0)
                            {
                                var oldPriceParentB = wait.Until(driver => mouse[i].FindElement(By.CssSelector("p.price > span.price-old")));
                                var priceIntB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_integer")));
                                var priceDecimalOldB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_decimal")));
                                oldPriceBeforeClick = priceIntB.Text + "." + priceDecimalOldB.Text;
                            }
                            else
                            {
                                oldPriceBeforeClick = "0";
                            }

                            var priceParentB = wait.Until(driver => mouse[i].FindElement(By.CssSelector(".price")));
                            var priceSpanB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span")).Last(e => e.GetAttribute("class") == "tb_integer"));
                            var priceDecimalB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span.tb_decimal")).Last());
                            string priceBeforeClick = priceSpanB.Text + "." + priceDecimalB.Text;
                            var NameBeforeClick = mouse[i].FindElement(By.CssSelector("div.caption > h4"));

                            if (await _fireBaseServices.checkIfAlreadyExists(endPoint, storeName, NameBeforeClick.Text, oldPriceBeforeClick, priceBeforeClick))
                            {
                                Console.WriteLine("The element and the price are already the same and exists");
                                storedproducts.Add(NameBeforeClick.Text);
                                continue;
                            }
                            else
                            {
                                Thread.Sleep(10000);
                                // Scroll the element into view using JavaScript
                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                js.ExecuteScript("arguments[0].scrollIntoView();", mouse[i]);
                                // Wait for the element to become clickable
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(mouse[i]));
                                //click on chair
                                actions.MoveToElement(mouse[i].FindElement(By.CssSelector("div.caption > h4 > a"))).Click().Build().Perform();
                                //get mouse name
                                var mouseName = wait.Until(driver => driver.FindElement(By.CssSelector(".tb_wt h1")));
                                //get mouse img
                                var mouseImage = wait.Until(driver => driver.FindElement(By.CssSelector("div.tb_slides > span > img")));
                                //get mouse price
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
                                //get mouse stock
                                var mouseStock = wait.Until(driver => driver.FindElements(By.CssSelector("span"))
                                                  .Where(e => e.GetAttribute("class").Contains("tb_stock_status"))
                                                  .FirstOrDefault());
                                //GET BRAND 
                                IWebElement productInfoSystem = driver.FindElement(By.Id("ProductInfoSystem_Ho7r8pnm"));

                                // Find all the dd elements within the div
                                IList<IWebElement> ddElements = productInfoSystem.FindElements(By.TagName("dd"));

                                // Get the text of the last dd element
                                string brand = ddElements[ddElements.Count - 1].Text;
                                //get desc
                                IWebElement divElement = driver.FindElement(By.Id("ProductFieldSystem_DnsPhn0S"));
                                liTexts.Add(divElement.Text);
                                //insert cpu into firebase
                                string productUrl = driver.Url;
                                await _fireBaseServices.insertDataIntoFirebase(endPoint, storeName, mouseName.Text, mouseImage.GetAttribute("src"), oldPrice, newPrice, mouseStock.Text, brand, liTexts, productUrl);
                                storedproducts.Add(mouseName.Text);
                                driver.Navigate().Back();
                                // clear the list
                                liTexts.Clear();
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".product-thumb")));
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div.caption > p.price")));
                                mouse = driver.FindElements(By.CssSelector(".product-layout > .product-thumb"));
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
                    catch (Exception ex)
                    {
                        // Handle other exceptions - e.g. log the error or show an error message
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                    try
                    {
                      var nextButton = driver.FindElement(By.CssSelector("li.next > a"));
                       if (nextButton != null)
                      {

                            actions.MoveToElement(nextButton).Click().Build().Perform();
                            Thread.Sleep(5000);
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

