// Initialize Firebase
var AuthSecret = "Au1R5GOJuXI654VqsOZYAlYatLlnylajYFoPu75v";
var BasePath = "https://test-811c3-default-rtdb.firebaseio.com";
var firebaseConfig = {
    
    apiKey: AuthSecret,
    databaseURL: BasePath
  };
firebase.initializeApp(firebaseConfig);
var url = window.location.href
const urlParams = new URLSearchParams(window.location.search);
const category = urlParams.get('category');
  // Create a reference to the database
var database = firebase.database();
var storeName;
var storeNameUrl = urlParams.get('storeName');
var componentsCounter = 0;

$(document).ready(function () {
    var existingProductIds = JSON.parse(localStorage.getItem('selectedProducts')) || [];
    console.log(urlParams);
    console.log(existingProductIds);
    database.ref().once("value", function (snapshot) {
        snapshot.forEach(function (childSnapshot) {
            var storeName = childSnapshot.key;
            var storeComponentsCounter = 0;
            database.ref(storeName).once("value", function (snapshot) {
                snapshot.forEach(function (childSnapshot) {
                  
                    var categoryName = childSnapshot.key;
                    database.ref(storeName + '/' + categoryName).once("value", function (snapshot) {
                        existingProductIds.forEach(function (productId) {
                            snapshot.forEach(function (childSnapshot) {
                                if (productId == childSnapshot.key) {
                                    $("#avalabileComponents").text(componentsCounter);
                                    var component = childSnapshot.val();
                                    console.log(childSnapshot.key);

                                    storeComponentsCounter++;
                                    componentsCounter++;
                                    // Skip components with OldPrice = 0
                                    var oldPrice = component.OldPrice.toString() === "0" ? "" : component.OldPrice;
                                    // Append component to component row
                                    $("#componentRow").append("<div class='productCol col-lg-2 col-md-4 col-sm-12 m-3 ' data-storeName=" + storeName + "   data-price=" + component.Price + ">  <div class='componentCard h-100 d-flex flex-column product'><div class='componentImg'><img src='" + component.Image + " ' class='img-fluid ' ></div><h1 class='text-center'>" + component.Name + "</h1><div class='prices d-flex justify-content-center'><span class='oldPrice' id='oldPrice'>" + oldPrice + "</span><span class='price'>" + component.Price + "JOD</span></div><div class='storeName'><span>" + storeName + "</span></div><div class='compareButton'><button class='compareButton' data-id=" + childSnapshot.key + ">Compare</button></div></div></div>");
                                }
                                });
                          

                        });

                    });

                    
                });
            });
        });
    });
    $(document).on('click', '.compareButton', function () {
        var productId = $(this).data("id");
        var existingProductIds = JSON.parse(localStorage.getItem('selectedProducts')) || [];
        if (existingProductIds.length === 2) {
            console.log(localStorage.getItem('selectedProducts'));
            return;
        }
        if (existingProductIds.includes(productId)) {
            // If product already exists in the array, do nothing
            return;
        }
        if (productId != null) {
            existingProductIds.push(productId);
            localStorage.setItem('selectedProducts', JSON.stringify(existingProductIds));

        }
    });

});
function showAllElements() {
    var row = $("#componentRow");
    var productCols = row.children('.productCol');
    $("#avalabileComponents").text(componentsCounter);
    productCols.show();
}