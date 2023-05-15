using Microsoft.AspNetCore.SignalR;
using test.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace test.smartBuy
{
    public class smartBuyTv_s : Hub, IsmartBuyTv_s
    {
        private HashSet<string> storedproducts = new HashSet<string>();
        string endPoint = "Products/Tv's/";
        string storeName = "SmartBuy";
        public async Task getTv()
        {
            List<string> liTexts = new List<string>();
            var chromeOptions = new ChromeOptions();
            var service = ChromeDriverService.CreateDefaultService(@"C:/Users/tsmra/Desktop/chromedriver.exe");
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("window-size=1920,1080");
            string regexPattern = @"[\d,.]+";
            using (var driver = new ChromeDriver(service, chromeOptions))
            {
                List<IWebElement> productList;
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                fireBaseServices _fireBaseServices = new fireBaseServices();
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 120));
                driver.Navigate().GoToUrl("https://smartbuy-me.com/smartbuystore/ar/%D8%A7%D9%84%D8%B4%D8%A7%D8%B4%D8%A7%D8%AA-%D9%88-%D8%A7%D8%AC%D9%87%D8%B2%D8%A9-%D8%A7%D9%84%D8%AA%D8%B1%D9%81%D9%8A%D9%87/%D8%A7%D9%84%D8%B4%D8%A7%D8%B4%D8%A7%D8%AA-%D9%88-%D8%A7%D8%AC%D9%87%D8%B2%D8%A9-%D8%A7%D9%84%D8%AA%D8%B1%D9%81%D9%8A%D9%87/c/403");
                Thread.Sleep(5000);
                Actions actions = new Actions(driver);
                bool flag = true;
                var url = "https://smartbuy-me.com/smartbuystore/ar/%D8%A7%D9%84%D8%B4%D8%A7%D8%B4%D8%A7%D8%AA-%D9%88-%D8%A7%D8%AC%D9%87%D8%B2%D8%A9-%D8%A7%D9%84%D8%AA%D8%B1%D9%81%D9%8A%D9%87/%D8%A7%D9%84%D8%B4%D8%A7%D8%B4%D8%A7%D8%AA-%D9%88-%D8%A7%D8%AC%D9%87%D8%B2%D8%A9-%D8%A7%D9%84%D8%AA%D8%B1%D9%81%D9%8A%D9%87/c/403";
                while (true)
                {
                    Thread.Sleep(2000);
                    driver.ExecuteScript("window.scrollBy(0, 500)");
                    long pageHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");
                    long windowHeight = (long)driver.ExecuteScript("return window.innerHeight");
                    long scrollPosition = (long)driver.ExecuteScript("return window.pageYOffset");

                    if (scrollPosition + windowHeight >= pageHeight)
                    {
                        Console.WriteLine("No more products!");
                        Thread.Sleep(2000);
                        actions.MoveToElement(driver.FindElement(By.Id("back_to_top"))).Click().Build().Perform();
                        break;
                    }
                }
                //     productList = wait.Until(driver => driver.FindElements(By.ClassName("product-item"))).ToList();
                //    int totalProducts = productList.Count;
                //Console.WriteLine(totalProducts + " number of products");
                while (flag)
                {
                    try
                    {

                        Thread.Sleep(5000);
                        var Tv = wait.Until(driver => driver.FindElements(By.CssSelector(".product-item")));
                        int initialTvCount = Tv.Count;
                        // Console.WriteLine(driver.FindElements(By.CssSelector(".product-item")).ToList().Count);
                        //loop through all mobiles using productList
                        for (int i = 0; i < initialTvCount; i++)
                        {

                            Console.WriteLine("number of Tv's " + initialTvCount);
                            Thread.Sleep(2000);
                            // actions.MoveToElement(productList[i]).Perform();
                            js.ExecuteScript("arguments[0].scrollIntoView();", Tv[i]);
                            string oldPriceBeforeClick = "0";
                            string priceBeforeClick = "0";
                            if (wait.Until(driver => Tv[i].FindElement(By.CssSelector("span.discountPrice"))).Displayed)
                            {
                                var oldPriceSpan = wait.Until(driver => Tv[i].FindElement(By.CssSelector("span.discountPrice")));
                                oldPriceBeforeClick = oldPriceSpan.Text;
                                priceBeforeClick = wait.Until(driver => Tv[i].FindElement(By.CssSelector("span.orignalPrice"))).Text;
                                Console.WriteLine("oldPrice");
                            }
                            else
                            {
                                priceBeforeClick = wait.Until(driver => Tv[i].FindElement(By.CssSelector("div.price"))).Text;
                                oldPriceBeforeClick = "0";
                                Console.WriteLine("no oldPrice");
                            }

                            var NameBeforeClick = wait.Until(driver => Tv[i].FindElement(By.CssSelector("a.name.hidden-lg")));
                            string NameBeforeClickText = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].innerHTML;", NameBeforeClick);
                            string orginalPrice = Regex.Match(oldPriceBeforeClick, regexPattern).Value;
                            string currentPrice = Regex.Match(priceBeforeClick, regexPattern).Value;
                            if (await _fireBaseServices.checkIfAlreadyExists(endPoint, storeName, NameBeforeClickText.Trim(), orginalPrice, currentPrice) && !string.IsNullOrEmpty(NameBeforeClickText.Trim()))
                            {
                                Console.WriteLine("The element and the price are already the same and exists");
                                storedproducts.Add(NameBeforeClickText.Trim());
                                continue;
                            }
                            else if (string.IsNullOrEmpty(NameBeforeClickText.Trim()))
                            {
                                Console.WriteLine("Element has no name");
                                continue;
                            }
                            else
                            {
                                Thread.Sleep(5000);
                                // Scroll the element into view using JavaScript

                                // Wait for the element to become clickable
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(Tv[i]));
                                //click on pc
                                // actions.MoveToElement(productElement).Click().Build().Perform();
                                Thread.Sleep(10000);
                                //  open the link in a new tab using JavaScript
                                driver.ExecuteScript("window.open(arguments[0]);", url + Tv[i].FindElement(By.CssSelector(".thumb")).GetAttribute("href"));

                                // switch to the new tab
                                driver.SwitchTo().Window(driver.WindowHandles.Last());

                                var name = wait.Until(driver => driver.FindElement(By.CssSelector(".product-details > div.name")));
                                var img = wait.Until(driver => driver.FindElement(By.CssSelector(".mz-figure > img")));
                                string price = "0";
                                string oldPrice = "0";
                                if (driver.FindElement(By.CssSelector("span.discountPrice")).Displayed)
                                {
                                    price = wait.Until(driver => driver.FindElement(By.CssSelector("span.orignalPrice")).Text);
                                    oldPrice = wait.Until(driver => driver.FindElement(By.CssSelector("span.discountPrice")).Text);
                                }
                                else
                                {
                                    price = wait.Until(driver => driver.FindElement(By.CssSelector("p.price")).Text);
                                    oldPrice = "0";
                                }
                                // Find the "colors-holder" div element
                                if (driver.FindElements(By.CssSelector("div.description")).Count > 0)
                                {
                                    string description = wait.Until(driver => driver.FindElement(By.CssSelector("div.description")).Text);
                                    liTexts.Add(description);
                                }
                                else
                                {
                                    liTexts.Add("No description");
                                }
                                Thread.Sleep(5000);
                                string productUrl = driver.Url;
                                string brand = "off-Brand";
                                string stockStatus = "In-Stock";
                                await _fireBaseServices.insertDataIntoFirebase(endPoint, storeName, name.Text, img.GetAttribute("src"), Regex.Match(oldPrice, regexPattern).Value, Regex.Match(price, regexPattern).Value, stockStatus, brand, liTexts, productUrl);
                                storedproducts.Add(name.Text);
                                Thread.Sleep(5000);
                                // close the tab
                                driver.Close();

                                // switch back to the original tab
                                driver.SwitchTo().Window(driver.WindowHandles.First());
                                // Console.WriteLine(totalProducts + " number of products");
                                Thread.Sleep(5000);
                                //scroll down to end of the page to get all elements becuase website is using lazy loading
                                //    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".main-content")));
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".product-item")));
                                //   mobile = wait.Until(driver => driver.FindElements(By.CssSelector(".product-item")));


                            }
                            if (i >= initialTvCount - 1)
                            {
                                await _fireBaseServices.checkIfElementIsStillExists(endPoint, storeName, storedproducts);
                                storedproducts.Clear();
                                Thread.Sleep(5000);
                                Console.WriteLine("Finshed");
                                flag = false;
                                driver.Quit();
                                break;
                                // set flag to false to end the while loop
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
                        Console.WriteLine("WebDriver error");
                        //  driver.Navigate().Refresh();

                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions - e.g. log the error or show an error message
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }
        }
    }
}
