using Microsoft.AspNetCore.SignalR;
using test.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace test.GTS
{
    public class GTSComputers : Hub, IGTSComputers
    {
        private HashSet<string> storedproducts = new HashSet<string>();
        string endPoint = "Products/Computers/";
        string storeName = "GTS";
        public async Task getComputers()
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
                driver.Navigate().GoToUrl("https://gts.jo/computers/custom-gaming-pc");
                Thread.Sleep(10000);
                Actions actions = new Actions(driver);
                try
                {
                    var stockStatusCHK = driver.FindElement(By.Id("bf-attr-s0_7_60"));
                    if (stockStatusCHK.Enabled)
                    {
                        IWebElement stockStatusButton = driver.FindElement(By.Id("bf-attr-s0_7_60"));
                        if (!stockStatusButton.Selected)
                        {
                            actions.MoveToElement(stockStatusButton).Click().Build().Perform();
                            Thread.Sleep(10000);
                        }
                    }
                    else if (!stockStatusCHK.Enabled)
                    {
                        Console.WriteLine("No in stock products");
                        driver.Quit();
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

                        var pc = driver.FindElements(By.CssSelector(".product-layout > .product-thumb"));
                        for (int i = 0; i < pc.Count(); i++)
                        {
                            Thread.Sleep(5000);
                            string oldPriceBeforeClick = "0";
                            if (wait.Until(driver => pc[i].FindElements(By.CssSelector("p.price > span.price-old"))).Count > 0)
                            {
                                var oldPriceParentB = wait.Until(driver => pc[i].FindElement(By.CssSelector("p.price > span.price-old")));
                                var priceIntB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_integer")));
                                var priceDecimalOldB = wait.Until(driver => oldPriceParentB.FindElement(By.CssSelector(".tb_decimal")));
                                oldPriceBeforeClick = priceIntB.Text + "." + priceDecimalOldB.Text;
                            }
                            else
                            {
                                oldPriceBeforeClick = "0";
                            }

                            var priceParentB = wait.Until(driver => pc[i].FindElement(By.CssSelector(".price")));
                            var priceSpanB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span")).Last(e => e.GetAttribute("class") == "tb_integer"));
                            var priceDecimalB = wait.Until(driver => priceParentB.FindElements(By.CssSelector("span.tb_decimal")).Last());
                            string priceBeforeClick = priceSpanB.Text + "." + priceDecimalB.Text;
                            var NameBeforeClick = pc[i].FindElement(By.CssSelector("div.caption > h4"));

                            if (await _fireBaseServices.checkIfAlreadyExists(endPoint,storeName, NameBeforeClick.Text, oldPriceBeforeClick, priceBeforeClick))
                            {
                                Console.WriteLine("The element and the price are already the same and exists");
                                storedproducts.Add(NameBeforeClick.Text);
                                continue;
                            }
                            else
                            {
                                // Scroll the element into view using JavaScript
                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                js.ExecuteScript("arguments[0].scrollIntoView();", pc[i]);
                                // Wait for the element to become clickable
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(pc[i].FindElement(By.CssSelector("div.caption > h4"))));
                                //click on pc
                                actions.MoveToElement(pc[i].FindElement(By.CssSelector("div.caption > h4"))).Click().Build().Perform();
                                Thread.Sleep(10000);
                                //get pc name
                                var pcName = wait.Until(driver => driver.FindElement(By.CssSelector(".tb_wt h1")));
                                //get pc img
                                var pcImage = wait.Until(driver => driver.FindElement(By.CssSelector("div.tb_slides > span > img")));
                                //get pc price

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
                                string price = priceSpan.Text + "." + priceDecimal.Text;
                                //get desc
                                // Find the table element
                                Thread.Sleep(5000);
                                if (driver.FindElements(By.CssSelector("table.table")).Count > 0)
                                {
                                    actions.MoveToElement(driver.FindElement(By.CssSelector("table.table"))).Click().Build().Perform();
                                    IWebElement table = wait.Until(driver => driver.FindElement(By.CssSelector("table.table")));
                                    ReadOnlyCollection<IWebElement> rows = wait.Until(driver => table.FindElements(By.CssSelector("tbody > tr ")));
                                    // Find all the td elements within the table
                                    foreach (IWebElement td in rows)
                                    {
                                        ReadOnlyCollection<IWebElement> cells = td.FindElements(By.TagName("td"));
                                        if (cells.Count > 0)
                                        {
                                            string heading = cells[0].Text;
                                            string value = cells[1].Text;

                                            var rowObject = new
                                            {
                                                Label = heading,
                                                Value = value
                                            };
                                            liTexts.Add(heading + " : " + value);

                                        }
                                    }
                                }
                                else
                                {
                                    liTexts.Add("No description");
                                }
                                //get pc stock
                                var pcStock = wait.Until(driver => driver.FindElements(By.CssSelector("span"))
                                                      .Where(e => e.GetAttribute("class").Contains("tb_stock_status"))
                                                      .FirstOrDefault());
                                var brand = wait.Until(driver => driver.FindElement(By.CssSelector(".product-info-brand-value > a")));
                                string productUrl = driver.Url;

                                //insert pc into firebase
                                await _fireBaseServices.insertDataIntoFirebase(endPoint, storeName, pcName.Text, pcImage.GetAttribute("src"), oldPrice, price, pcStock.Text, brand.Text, liTexts, productUrl);
                                storedproducts.Add(pcName.Text);
                                driver.Navigate().Back();
                                // clear the list
                                liTexts.Clear();
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".product-layout > .product-thumb")));
                                pc = driver.FindElements(By.CssSelector(".product-layout > .product-thumb "));
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
                        var nextButton = driver.FindElement(By.CssSelector("ul.links > li > a[rel='next']"));
                        if (nextButton != null)
                        {
                            nextButton.Click();
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
