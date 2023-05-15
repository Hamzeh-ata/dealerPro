using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using static IronPython.Modules.PythonDateTime;

namespace test
{
    public class fireBaseServices
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "Au1R5GOJuXI654VqsOZYAlYatLlnylajYFoPu75v",
            BasePath = "https://test-811c3-default-rtdb.firebaseio.com"
        };
        IFirebaseClient client;
        private Dictionary<string, dynamic> _data;
        DateTime today = DateTime.Today;
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        public async Task insertDataIntoFirebase(string endPoint,string storeName,string name, string img , string oldPrice, string price, string socket, string brand,  List<string> LiTexts, string productUrl)
        {
           
            string formattedDate = today.ToString("yyyy-MM-dd");
            string formattedTime = DateTime.Now.ToString("HH:mm");
            client = new FirebaseClient(config);
            var response = await client.GetAsync(endPoint);
            _data = response.ResultAs<Dictionary<string, dynamic>>();
            bool containsItem = false;
            if(_data != null) {
                foreach (var item in _data)
                {
                    if (item.Value.StoreName == storeName && item.Value.Name == name)
                    {
                        containsItem = true;
                        break;
                    }
                }
            }
            


            if (_data == null)
            {
                //await wait for push each element then back to the function
                await client.PushAsync(endPoint, new { StoreName=storeName ,Name = name, Image = img,OldPrice = oldPrice, Price = price, socketStatus = socket,  Brand = brand  ,  description = LiTexts, Date = formattedDate, Time = formattedTime, ProductUrl = productUrl });
                return;
            }
            else if (!containsItem)
            {
                await client.PushAsync(endPoint, new { StoreName = storeName, Name = name, Image = img, OldPrice = oldPrice, Price = price, socketStatus = socket, Brand = brand, description = LiTexts, Date = formattedDate, Time = formattedTime, ProductUrl = productUrl });
                return;
            }
            else
            {
                //if the data sent is not dublicated then insert it else dont insert it.
                if (await checkForDublicate(name, storeName))
                {
                    await client.PushAsync(endPoint, new { StoreName = storeName, Name = name, Image = img, OldPrice = oldPrice, Price = price, socketStatus = socket, Brand = brand, description = LiTexts, Date = formattedDate, Time = formattedTime, ProductUrl = productUrl });

                }
                else if(!await checkForDublicate(name, storeName))
                {
                    var existingItem = _data.FirstOrDefault(x => x.Value.Name == name && x.Value.StoreName == storeName);
                    var existingKey = existingItem.Key;
                //    await client.UpdateAsync(endPoint + "/" + existingKey, new { Date = formattedDate, Time = formattedTime });

                    if (await checkForPriceChanges(name,storeName ,oldPrice, price))
                    {
                        existingItem = _data.FirstOrDefault(x => x.Value.Name == name && x.Value.StoreName == storeName);
                         existingKey = existingItem.Key;
                        Console.WriteLine("Price Not same");
                        await client.UpdateAsync(endPoint + "/" + existingKey, new { StoreName = storeName, Name = name, Image = img, OldPrice = oldPrice, Price = price, socketStatus = socket, Brand = brand, description = LiTexts, Date = formattedDate, Time = formattedTime, ProductUrl = productUrl });

                    }
                    else if(!await checkForPriceChanges(name, storeName,oldPrice, price))
                    {
                         existingItem = _data.FirstOrDefault(x => x.Value.Name == name && x.Value.StoreName == storeName);
                         existingKey = existingItem.Key;
                        Console.WriteLine("Already Exists and price is same");
                      await client.UpdateAsync(endPoint + "/" + existingKey, new { Date = formattedDate,Time = formattedTime });

                        return;
                       
                    }
                    return;
                   
                }
            }
        }
        //HMG
        public async Task insertDataIntoFirebase(string endPoint,string storeName ,string name, string img, string oldPrice, string price, string brand, List<string> LiTexts, string productUrl,string productNumber)
        {
            string formattedDate = today.ToString("yyyy-MM-dd");
            string formattedTime = DateTime.Now.ToString("HH:mm");
            client = new FirebaseClient(config);
            var response = await client.GetAsync(endPoint);
            _data = response.ResultAs<Dictionary<string, dynamic>>();
            bool containsItem = false;

            if (_data != null)
            {
                foreach (var item in _data)
                {
                    if (item.Value.StoreName == storeName && item.Value.ProductNumber == productNumber)
                    {
                        containsItem = true;
                        break;
                    }

                }
            }
            if (_data == null)
            {
                //await wait for push each element then back to the function
                await client.PushAsync(endPoint, new { StoreName =  storeName, Name = name, Image = img, OldPrice = oldPrice, Price = price, Brand = brand, description = LiTexts, ProductUrl = productUrl, ProductNumber = productNumber, Date = formattedDate, Time = formattedTime });
                return;
            }
            else if (!containsItem)
            {
                await client.PushAsync(endPoint, new { StoreName = storeName, Name = name, Image = img, OldPrice = oldPrice, Price = price, Brand = brand, description = LiTexts, ProductUrl = productUrl, ProductNumber = productNumber, Date = formattedDate, Time = formattedTime });
                return;
            }
            else
            {
                //if the data sent is not dublicated then insert it else dont insert it.
                if (await checkForDublicateHmg(productNumber,storeName))
                {
                    await client.PushAsync(endPoint, new { StoreName = storeName, Name = name, Image = img, OldPrice = oldPrice, Price = price, Brand = brand, description = LiTexts, ProductUrl = productUrl,ProductNumber = productNumber, Date = formattedDate, Time = formattedTime });

                }
                else if (!await checkForDublicateHmg(productNumber, storeName))
                {
                    var existingItem = _data.FirstOrDefault(x => x.Value.ProductNumber == productNumber && x.Value.StoreName == storeName);
                    var existingKey = existingItem.Key;
                    //    await client.UpdateAsync(endPoint + "/" + existingKey, new { Date = formattedDate, Time = formattedTime });

                    if (await HMGcheckForPriceChanges(productNumber, storeName,oldPrice, price))
                    {
                        existingItem = _data.FirstOrDefault(x => x.Value.ProductNumber == productNumber && x.Value.StoreName == storeName);
                        existingKey = existingItem.Key;
                        Console.WriteLine("Price Not same");
                        await client.UpdateAsync(endPoint + "/" + existingKey, new { StoreName = storeName, Name = name, Image = img, OldPrice = oldPrice, Price = price, Brand = brand, description = LiTexts, ProductUrl = productUrl, ProductNumber = productNumber, Date = formattedDate, Time = formattedTime });

                    }
                    else if (!await HMGcheckForPriceChanges(productNumber,storeName, oldPrice, price))
                    {
                        existingItem = _data.FirstOrDefault(x => x.Value.ProductNumber == productNumber && x.Value.StoreName == storeName);
                        existingKey = existingItem.Key;
                        Console.WriteLine("Already Exists and price is same");
                        await client.UpdateAsync(endPoint + "/" + existingKey, new { Date = formattedDate, Time = formattedTime });

                        return;

                    }
                    return;

                }
            }
        }
        private async Task<bool> checkForDublicate(string name,string storeName)
        {
            foreach (var item in _data)
            {
                var existingName = item.Value.Name;
                var existingStoreName = item.Value.StoreName;
                if (existingName == name && existingStoreName == storeName)
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<bool> checkForDublicateHmg(string ProductNumber,string StoreName)
        {
            foreach (var item in _data)
            {
                var existingImage = item.Value.ProductNumber;
                var existingStoreName = item.Value.StoreName;
                if (existingImage == ProductNumber && existingStoreName == StoreName)
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<bool>checkForPriceChanges(string name, string storeName, string oldPrice,string price)
        {
            foreach (var item in _data)
            {
                var existingName = item.Value.Name;
                var existingproductStoreName = item.Value.StoreName;
                if (existingName == name && existingproductStoreName == storeName)
                {
                    var existingPrice = item.Value.Price;
                    var existingOldPrice = item.Value.OldPrice;
                    if (existingPrice != price)
                    {
                       // Console.WriteLine("Current PRICE NOT SAME");
                        Console.WriteLine("Current price before change : " + existingPrice + "Current price after changes : " + price);
                        return true;
                    }
                    if (existingOldPrice != oldPrice)
                    {
                        Console.WriteLine("OLD PRICE NOT SAME");
                        return true;
                    }
                  
                    return false;
                }

            }
            Console.WriteLine("ITEM NOT FOUND");
            return false;
        }
        private async Task<bool> HMGcheckForPriceChanges(string productNumber,string storeName, string oldPrice, string price)
        {
            foreach (var item in _data)
            {
                var existingproductNumber = item.Value.ProductNumber;
                var existingproductStoreName = item.Value.StoreName;
                if (existingproductNumber == productNumber && existingproductStoreName== storeName)
                {
                    var existingPrice = item.Value.Price;
                    var existingOldPrice = item.Value.OldPrice;
                    if (existingPrice != price)
                    {
                        // Console.WriteLine("Current PRICE NOT SAME");
                        Console.WriteLine("Current price before change : " + existingPrice + "Current price after changes : " + price);
                        return true;
                    }
                    if (existingOldPrice != oldPrice)
                    {
                        Console.WriteLine("OLD PRICE NOT SAME");
                        return true;
                    }

                    return false;
                }

            }
            Console.WriteLine("ITEM NOT FOUND");
            return false;
        }
        public async Task checkIfElementIsStillExists(string endPoint,string storeName, HashSet<string> storedProducts)
        {
            if (_data != null && storedProducts.Count>0)
            {

                foreach (var item in _data)
                {
                    if (item.Value.StoreName == storeName) { 
                        var existingName = item.Value.Name;
                    var existingStoreName = item.Value.StoreName;
                    if (!string.IsNullOrEmpty(existingName.ToString()) && !storedProducts.Contains(existingName.ToString()))
                    {
                        Console.WriteLine(existingName + " Deleted");
                        await client.DeleteAsync(endPoint + "/" + item.Key);
                    }
                }
                }
            }
        }
        public async Task HMGcheckIfElementIsStillExists(string endPoint, string storeName,HashSet<string> storedProducts)
        {
            if (_data != null && storedProducts.Count > 0)
            {
                foreach (var item in _data)
                {
                    if (item.Value.StoreName == storeName)
                    {
                        var existingProductNumber = item.Value.ProductNumber;
                        var existingStoreName = item.Value.StoreName;
                        Console.WriteLine("the productstored is " + existingProductNumber + existingStoreName);
                        if (!string.IsNullOrEmpty(existingProductNumber.ToString()) && !storedProducts.Contains(existingProductNumber.ToString()))
                        {
                            Console.WriteLine(existingProductNumber + " Deleted");
                            await client.DeleteAsync(endPoint + "/" + item.Key);
                        }
                    }
                }
            }
        }
        public async Task<bool> checkIfAlreadyExists(string endPoint,string storeName, string name, string oldPrice, string price)
        {

            client = new FirebaseClient(config);
            string formattedDate = today.ToString("yyyy-MM-dd");
            string formattedTime = DateTime.Now.ToString("HH:mm");
            var response = await client.GetAsync(endPoint);
            _data = response.ResultAs<Dictionary<string, dynamic>>();
            if (_data != null)
            {
                foreach (var item in _data)
                {
                    var existingItem = _data.FirstOrDefault(x => x.Value.Name == name);
                    var existingKey = existingItem.Key;
                    var existingName = item.Value.Name;
                    var existingPrice = item.Value.Price;
                    var existingOldPrice = item.Value.OldPrice;
                    var existingStoreName = item.Value.StoreName;
                    if (name == existingName.ToString() && oldPrice == existingOldPrice.ToString() && price == existingPrice.ToString() && storeName== existingStoreName.ToString())
                    {
                        await client.UpdateAsync(endPoint + "/" + existingKey, new { Date = formattedDate, Time = formattedTime });
                        return true;
                    }
                }
            }

            return false;
        }


    }
}
