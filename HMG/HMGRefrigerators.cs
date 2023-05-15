﻿using Microsoft.AspNetCore.SignalR;
using test.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace test.HMG
{
    public class HMGRefrigerators : Hub, IHMGRefrigerators
    {
        private HashSet<string> storedproducts = new HashSet<string>();
        string endPoint = "Products/Refrigerators/";
        string storeName = "HMG";
        public async Task getRefrigerator()
        {
            var chromeOptions = new ChromeOptions();
            var service = ChromeDriverService.CreateDefaultService(@"C:/Users/tsmra/Desktop/chromedriver.exe");
            List<string> liTexts = new List<string>();
            bool flag = true;
            // matches any sequence of digits, commas, and dots
            string regexPattern = @"[\d,.]+";
            //   chromeOptions.AddArgument("--headless");
            //  chromeOptions.AddArgument("--disable-gpu");
            //  chromeOptions.AddArgument("--no-sandbox");
            //  chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("window-size=1920,1080");
            using (var driver = new ChromeDriver(service, chromeOptions))
            {
                fireBaseServices _fireBaseServices = new fireBaseServices();
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 120));
                driver.Navigate().GoToUrl("https://hmg.jo/product-category/refrigerators/");
                Thread.Sleep(12000);
                Actions actions = new Actions(driver);

                try
                {
                    Thread.Sleep(5000);
                    // Find the select element by its name
                    var selectElement = wait.Until(driver => driver.FindElement(By.CssSelector(".electro-wc-wppp-select")));
                    // Use SelectElement to wrap the select element
                    var select = new SelectElement(selectElement);
                    // Select the option with value "-1"
                    select.SelectByValue("-1");
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
                while (flag)
                {
                    try
                    {
                        Thread.Sleep(5000);
                        var Refrigerator = driver.FindElements(By.CssSelector(".site-main > ul.products li.product > .product-item__outer > .product-item__inner "));
                        int RefrigeratorCount = Refrigerator.Count;
                        for (int i=0; i < Refrigerator.Count(); i++)
                        {
                         
                                Thread.Sleep(5000);
                            // Scroll the element into view using JavaScript
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(Refrigerator[i]));

                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'auto', block: 'center', inline: 'center'});", Refrigerator[i]);
                                // Wait for the element to become clickable
                                Thread.Sleep(5000);
                                actions.MoveToElement(Refrigerator[i].FindElement(By.CssSelector(".woocommerce-loop-product__link"))).Click().Build().Perform();
                                Thread.Sleep(10000);
                                var Name = wait.Until(driver => driver.FindElement(By.CssSelector(".summary > h1.product_title")));
                                Console.WriteLine("name after click " + Name.Text);
                                var Image = wait.Until(driver => driver.FindElement(By.CssSelector(".woocommerce-product-gallery__image > img.wp-post-image")));
                                Console.WriteLine("name after click " + Image.GetAttribute("src"));
                                string oldPrice = "0";
                                string currentPrice = "0";
                                if (wait.Until(driver => driver.FindElements(By.CssSelector(".entry-summary p.price > span.electro-price del > .woocommerce-Price-amount > bdi "))).Count > 0)
                                {                                                                                       
                                 
                                    oldPrice = wait.Until(driver => driver.FindElement(By.CssSelector(".entry-summary p.price > span.electro-price del > .woocommerce-Price-amount > bdi "))).Text;
                                    currentPrice = wait.Until(driver => driver.FindElement(By.CssSelector(".entry-summary p.price > span.electro-price ins > .woocommerce-Price-amount > bdi "))).Text;
                                                                                                                          
                                }
                                else
                                {
                                    oldPrice = "0";
                                    currentPrice = wait.Until(driver => driver.FindElement(By.CssSelector(".entry-summary p.price > span.electro-price > .woocommerce-Price-amount > bdi "))).Text;
                                }
                                string currentPriceWithoutCurrency = Regex.Match(currentPrice, regexPattern).Value;
                                string oldPriceWithoutCurrency = Regex.Match(oldPrice, regexPattern).Value;
                                var brand= wait.Until(driver => driver.FindElement(By.CssSelector(".entry-summary .brand img"))).GetAttribute("alt");
                                 string productNumber = wait.Until(driver => driver.FindElement(By.CssSelector(".entry-summary > .woocommerce-product-details__short-description"))).Text;
                         

                            string productUrl = driver.Url;
                                if (driver.FindElements(By.Id("tab-title-specification")).Count > 0)
                                {
                                    Thread.Sleep(5000);
                                    var element = driver.FindElement(By.CssSelector("li.specification_tab "));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'auto', block: 'center', inline: 'center'});", element);
                                    Thread.Sleep(5000);
                                    element.Click();
                                    Thread.Sleep(5000);
                                    // wait for the table to be visible
                                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("tab-specification")));
                                    var table = driver.FindElement(By.Id("tab-specification"));
                                    // get all the th and td elements in the table and save them as a list of objects
                                    var rows = table.FindElements(By.TagName("tr"));
                                    var dataList = new List<object>();

                                    foreach (var row in rows)
                                    {
                                        var cells = row.FindElements(By.TagName("td"));
                                        var headers = row.FindElements(By.TagName("th"));
                                        liTexts.Add(headers[0].Text + " : " + cells[0].Text);
                                    }
                                }
                                else
                                {
                                    liTexts.Add("No description");
                                }

                                await _fireBaseServices.insertDataIntoFirebase(endPoint,storeName, Name.Text, Image.GetAttribute("src"), oldPriceWithoutCurrency.Trim(), currentPriceWithoutCurrency.Trim(), brand, liTexts, productUrl,productNumber);
                            storedproducts.Add(productNumber + ":" + storeName);
                            driver.Navigate().Back();
                                liTexts.Clear();
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".site-main > ul.products li.product")));
                                Refrigerator = driver.FindElements(By.CssSelector(".site-main > ul.products li.product > .product-item__outer > .product-item__inner"));
                            if (i >= RefrigeratorCount - 1)
                            {
                                await _fireBaseServices.HMGcheckIfElementIsStillExists(endPoint, storeName, storedproducts); storedproducts.Clear();
                                Thread.Sleep(5000);
                                Console.WriteLine("Finshed");
                                flag = false;
                                break;
                                driver.Quit(); // set flag to false to end the while loop
                            }
                        }
                    }
                    catch (NoSuchElementException ex)
                    {
                        Console.WriteLine("NoSuchElementExceptionn: " + ex.Message);
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
                }
            }
             
        }
    }
}
