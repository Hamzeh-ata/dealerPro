using Microsoft.AspNetCore.SignalR;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using test.Interfaces;

namespace test.Hubs
{
    public class cityCenterHDD : Hub, IcityCenterHDD
    {
        private HashSet<string> storedproducts = new HashSet<string>();
        string endPoint = "Products/HDD/";
        string storeName = "CityCenter";
        public async Task GetHDD()
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
                driver.Navigate().GoToUrl("https://citycenter.jo/gaming/gaming-storage-drive/hard-drives");
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
                        var HDD = wait.Until(driver => driver.FindElements(By.CssSelector(".product-layout > .product-thumb")));
                        for (int i = 0; i < HDD.Count(); i++) // loop through all CPUs except the last one
                        {
                            Thread.Sleep(5000);
                            string oldPriceBeforeClick = "0";
                            if (wait.Until(driver => HDD[i].FindElements(By.CssSelector("p.price > span.price-old"))).Count > 0)
                            {
                                var oldPriceParentB = wait.Until(driver => HDD[i].FindElement(By.CssSelector("p.price > span.price-old")));
                                var priceIntB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_integer")));
                                var priceDecimalOldB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_decimal")));
                                oldPriceBeforeClick = priceIntB.Text + "." + priceDecimalOldB.Text;
                            }
                            else
                            {
                                oldPriceBeforeClick = "0";
                            }

                            var priceParentB = wait.Until(driver => HDD[i].FindElement(By.CssSelector(".price")));
                            var priceSpanB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span")).Last(e => e.GetAttribute("class") == "tb_integer"));
                            var priceDecimalB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span.tb_decimal")).Last());
                            string priceBeforeClick = priceSpanB.Text + "." + priceDecimalB.Text;
                            var NameBeforeClick = HDD[i].FindElement(By.CssSelector("div.caption > h4"));
                            storedproducts.Add(NameBeforeClick.Text);
                            if (await _fireBaseServices.checkIfAlreadyExists(endPoint, storeName, NameBeforeClick.Text, oldPriceBeforeClick, priceBeforeClick))
                            {
                                Console.WriteLine("The element and the price are already the same and exists");
                                continue;
                            }
                            else
                            {
                                // Scroll the element into view using JavaScript
                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                js.ExecuteScript("arguments[0].scrollIntoView();", HDD[i]);
                                // Wait for the element to become clickable
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(HDD[i]));
                                //click on hdd
                                actions.MoveToElement(HDD[i].FindElement(By.CssSelector("div.caption > h4 > a"))).Click().Build().Perform();
                                //get hdd name
                                Thread.Sleep(10000);
                                var hddName = wait.Until(driver => driver.FindElement(By.CssSelector(".tb_wt h1")));
                                //get psu img
                                var hddImage = wait.Until(driver => driver.FindElement(By.CssSelector("div.tb_slides > span > img")));
                                //get SSD price
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
                                //get hdd stock
                                var hddStock = wait.Until(driver => driver.FindElements(By.CssSelector("span"))
                                                      .Where(e => e.GetAttribute("class").Contains("tb_stock_status"))
                                                      .FirstOrDefault());
                                //GET BRAND 
                                // Locate the div element by its ID
                                IWebElement productInfoSystem = driver.FindElement(By.Id("ProductInfoSystem_Ho7r8pnm"));

                                // Find all the dd elements within the div
                                IList<IWebElement> ddElements = productInfoSystem.FindElements(By.TagName("dd"));

                                // Get the text of the last dd element
                                string brand = ddElements[ddElements.Count - 1].Text;

                                //get desc
                                IWebElement divElement = driver.FindElement(By.Id("ProductFieldSystem_DnsPhn0S"));
                                liTexts.Add(divElement.Text);
                                //insert hdd into firebase
                                string productUrl = driver.Url;
                                await _fireBaseServices.insertDataIntoFirebase(endPoint, storeName, hddName.Text, hddImage.GetAttribute("src"), oldPrice, newPrice, hddStock.Text, brand, liTexts, productUrl);
                                driver.Navigate().Back();
                                // clear the list
                                liTexts.Clear();
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".product-thumb")));
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div.caption > p.price")));
                                HDD = driver.FindElements(By.CssSelector(".product-layout > .product-thumb"));
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
                            Thread.Sleep(10000);
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
