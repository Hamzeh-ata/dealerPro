using Microsoft.AspNetCore.SignalR;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using test.Interfaces;

namespace test.carrefour
{
    public class carrefourDishWashers:Hub ,IcarrefourDishWashers
    {
            private HashSet<string> storedproducts = new HashSet<string>();
            string endPoint = "Products/Dishwashers/";
            string storeName = "Carrefour";
            int scrollCounter = 0;
            public async Task getDishWasher()
            {

                var chromeOptions = new ChromeOptions();
                var service = ChromeDriverService.CreateDefaultService(@"D:/chromeDriver/chromedriver.exe");
                List<string> liTexts = new List<string>();
                // matches any sequence of digits, commas, and dots
                   chromeOptions.AddArgument("--headless");
                   chromeOptions.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                   chromeOptions.AddArgument("--enable-javascript");
                   chromeOptions.AddArgument("--enable-automation");
                   chromeOptions.AddArgument("--disable-gpu");
                   chromeOptions.AddArgument("--no-sandbox");
                   chromeOptions.AddArgument("--proxy-server");
                   chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--disable-extensions");
                   chromeOptions.AddArgument("--enable-features=NetworkService,NetworkServiceInProcess");
                   chromeOptions.AddArgument("--disable-web-security");
                chromeOptions.AddArgument("window-size=1920,1080");
                string regexPattern = @"[\d,.]+";
                using (var driver = new ChromeDriver(service, chromeOptions))
                {
                    fireBaseServices _fireBaseServices = new fireBaseServices();
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 120));
                    driver.Navigate().GoToUrl("https://www.carrefourjordan.com/mafjor/en/c/NFJOR4100300");
                    Thread.Sleep(12000);
                    Actions actions = new Actions(driver);
                    bool flag = true;
                    int i = 0;
                    while (true)
                    {
                        Thread.Sleep(2000);
                        driver.ExecuteScript("window.scrollBy(0, 500)");
                        long pageHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");
                        long windowHeight = (long)driver.ExecuteScript("return window.innerHeight");
                        long scrollPosition = (long)driver.ExecuteScript("return window.pageYOffset");
                        wait.Until(driver => driver.FindElements(By.CssSelector(".css-b9nx4o")));
                        if (driver.FindElements(By.CssSelector("div.css-1hbp62g button[data-testid='trolly-button']")).Count > 0)
                        {
                            IJavaScriptExecutor jsScroll = (IJavaScriptExecutor)driver;
                            // Find the element by class and data-testid
                            IWebElement loadMoreButton = driver.FindElement(By.CssSelector("div.css-1hbp62g button[data-testid='trolly-button']"));
                            jsScroll.ExecuteScript("arguments[0].scrollIntoView();", loadMoreButton);
                            // Click the element
                            loadMoreButton.Click();
                            Thread.Sleep(2000);
                        }
                        if (scrollPosition + windowHeight >= pageHeight)
                        {
                            scrollCounter++;
                            actions.MoveToElement(driver.FindElement(By.ClassName("css-1hpnayn"))).Click().Build().Perform();
                            Thread.Sleep(2000);
                        }
                        if (scrollPosition + windowHeight >= pageHeight && scrollCounter >= 2)
                        {
                            Thread.Sleep(2000);
                            actions.MoveToElement(driver.FindElement(By.ClassName("css-1hpnayn"))).Click().Build().Perform();
                            break;
                        }
                    }



                    while (flag)
                    {
                        var Dishwasher = wait.Until(driver => driver.FindElements(By.CssSelector(".css-b9nx4o")));
                        var productsCount = Dishwasher.Count();
                      
                        try
                        {
                            //Thread.Sleep(5000);
                        for (; i < productsCount; i++)
                            {
                                Console.WriteLine("number of products " + productsCount + " number of i " + i);
                                js.ExecuteScript("arguments[0].scrollIntoView();", Dishwasher[i]);

                                Thread.Sleep(5000);
                                string orginalPriceBeforeClick = "0";
                                string currentPrice = "0";
                                string orginalPriceBeforeClickWithoutCurrency = "0";
                                if (wait.Until(driver => Dishwasher[i].FindElements(By.CssSelector(".css-iqeby6"))).Count > 0)
                                {
                                    orginalPriceBeforeClick = wait.Until(driver => Dishwasher[i].FindElement(By.CssSelector(".css-iqeby6 > .css-17fvam3"))).Text.Replace(",", ""); ;
                                    currentPrice = wait.Until(driver => Dishwasher[i].FindElement(By.CssSelector(".css-2a09gr > .css-17fvam3"))).Text.Replace(",", ""); ;
                                }
                                else
                                {
                                    currentPrice = wait.Until(driver => Dishwasher[i].FindElement(By.CssSelector(".css-fzp91j > .css-17fvam3"))).Text.Replace(",", ""); ;
                                    orginalPriceBeforeClick = "0";
                                }
                                string currentPriceBeforeClickWithoutCurrency = Regex.Match(currentPrice + 0, regexPattern).Value;
                                if (orginalPriceBeforeClick != "0")
                                {
                                    orginalPriceBeforeClickWithoutCurrency = Regex.Match((orginalPriceBeforeClick + 0), regexPattern).Value.Replace(",", ""); ;

                                }
                                string NameBeforeClick = Dishwasher[i].FindElement(By.CssSelector(".css-11qbfb > .css-1nhiovu > a")).Text;
                                Console.WriteLine("org price before click " + orginalPriceBeforeClickWithoutCurrency + ", current price before click" + currentPriceBeforeClickWithoutCurrency);
                                if (await _fireBaseServices.checkIfAlreadyExists(endPoint, storeName,NameBeforeClick, orginalPriceBeforeClickWithoutCurrency, currentPriceBeforeClickWithoutCurrency))
                                {
                                    Console.WriteLine("The element and the price are already the same and exists");
                                    storedproducts.Add(NameBeforeClick);
                                    continue;
                                }
                                else
                                {
                                    // Scroll the element into view using JavaScript

                                      js.ExecuteScript("arguments[0].scrollIntoView();", Dishwasher[i]);
                                    // Wait for the element to become clickable
                                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(Dishwasher[i]));

                                    driver.ExecuteScript("window.open(arguments[0]);", Dishwasher[i].FindElement(By.CssSelector(".css-1nhiovu > a")).GetAttribute("href"));
                                    // switch to the new tab
                                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                                    Thread.Sleep(10000);
                                    var Name = wait.Until(driver => driver.FindElement(By.CssSelector("main.css-z4qeze .css-1cvrzon > .css-q7lfpb > section.css-168eikn > h1.css-106scfp")));
                                    var Image = wait.Until(driver => driver.FindElement(By.CssSelector("main.css-z4qeze > .css-peby8 > .css-1cvrzon > .css-1spgxsy > section.css-18qa9gz  .css-1c2pck7 > div > img")));
                                    string oldPrice = "0";
                                    if (wait.Until(driver => driver.FindElements(By.CssSelector("section.css-168eikn > .css-1oh8fze > h2.css-1i90gmp > span.css-rmycza > del.css-1bdwabt"))).Count > 0)
                                    {

                                        oldPrice = wait.Until(driver => driver.FindElement(By.CssSelector("section.css-168eikn  > .css-1oh8fze > h2.css-1i90gmp > span.css-rmycza > del.css-1bdwabt"))).Text;
                                        currentPrice = wait.Until(driver => driver.FindElement(By.CssSelector("section.css-168eikn > .css-1oh8fze > h2.css-1i90gmp"))).Text;

                                    }
                                    else
                                    {
                                        currentPrice = wait.Until(driver => driver.FindElement(By.CssSelector("section.css-168eikn > .css-1oh8fze > h2.css-17ctnp"))).Text;

                                        oldPrice = "0";
                                    }
                                    string oldPricePriceClickWithoutCurrency = Regex.Match(oldPrice, regexPattern).Value;
                                    string currentPriceClickWithoutCurrency = Regex.Match(currentPrice, regexPattern).Value;
                                    string brand = "";
                                    if (wait.Until(driver => driver.FindElements(By.CssSelector(".css-1khiat5 "))).Count > 0)
                                    {
                                        Console.WriteLine("Ther is brand");
                                        brand = wait.Until(driver => driver.FindElement(By.CssSelector("section.css-168eikn  > .css-1khiat5  > div.css-1bdwabt > a.css-1nnke3o"))).Text;
                                    }
                                    else
                                    {
                                        brand = "off-brand";

                                    }
                                    if (wait.Until(driver => driver.FindElements(By.CssSelector(".css-d6evrn > div.css-3vwsxh"))).Count > 0)
                                    {
                                        IWebElement parentDiv = driver.FindElement(By.CssSelector("div.css-i1r47t"));

                                        // Find all the child div and h3 elements
                                        IList<IWebElement> divElements = parentDiv.FindElements(By.CssSelector("div.css-pi51ey"));
                                        IList<IWebElement> h3Elements = parentDiv.FindElements(By.CssSelector("h3.css-1ps12pz"));

                                        // Loop through the elements and add them to the list as objects
                                        for (int f = 0; f < divElements.Count; f++)
                                        {
                                            string Heading = divElements[f].Text;
                                            string Value = h3Elements[f].Text;
                                            // Create an object with the div element's text as the heading and the h3 element's text as the value
                                            liTexts.Add(Heading + " : " + Value);
                                        }
                                    }
                                    else if (wait.Until(driver => driver.FindElements(By.CssSelector(".css-d6evrn > .css-1weog53"))).Count > 0)
                                    {
                                        string description = wait.Until(driver => driver.FindElement(By.CssSelector(".css-d6evrn > .css-1weog53"))).Text;
                                        liTexts.Add(description);
                                    }
                                    else
                                    {
                                        liTexts.Add("No description");

                                    }
                                    string productUrl = driver.Url;
                                    string socket = "In socket";
                                    Thread.Sleep(5000);
                                    await _fireBaseServices.insertDataIntoFirebase(endPoint, storeName, Name.Text, Image.GetAttribute("src"), oldPricePriceClickWithoutCurrency, currentPriceClickWithoutCurrency, socket, brand, liTexts, productUrl);
                                    storedproducts.Add(Name.Text);
                                    Thread.Sleep(5000);
                                    liTexts.Clear();
                                    // close the tab
                                    driver.Close();

                                    // switch back to the original tab
                                    driver.SwitchTo().Window(driver.WindowHandles.First());
                                    // Console.WriteLine(totalProducts + " number of products");
                                    Thread.Sleep(5000);
                                    //scroll down to end of the page to get all elements becuase website is using lazy loading
                                    //    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".main-content")));
                                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".css-b9nx4o")));
                                }
                                if (i >= productsCount - 1)
                                {

                                    break;


                                }
                            }
                        if (i >= productsCount)
                        {
                            await _fireBaseServices.checkIfElementIsStillExists(endPoint, storeName, storedproducts);
                            storedproducts.Clear();
                            Thread.Sleep(5000);
                            Console.WriteLine("Finshed");
                            flag = false;
                            driver.Quit(); // end the while loop
                            break;
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

