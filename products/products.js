var url = window.location.href
var urlParams = new URLSearchParams(window.location.search);
const category = urlParams.get('category');
const searchQuery = urlParams.get('search');
var storeNameUrl = urlParams.get('storeName');
var removeButtons = $('.removeProductFromComparimation');
var dropdownBtn = $('#dropdown-btn');
var dropdownContent = $('#dropdown-content');
const storesName = new Set();
var storeCounts = {};
var totalComponentsCounter = 0;
storesName.add("s");
// Store the fetched components in a cache
$(document).ready(function () {
    urlParams = new URLSearchParams(window.location.search);
    storeNameUrl = urlParams.get('storeName');
    var selectedProducts = localStorage.getItem('selectedProducts');
    if (selectedProducts != null) {
        localStorage.removeItem('selectedProducts');
    }
    if (category) {
        loadItems();
     
    }
        // Call the loadNextSet function to fetch and append the first set of items.
    else if (searchQuery) {
        loadItemsBySearch(searchQuery);
        componentsCounter = 0;
        if (storeNameUrl) {
            filterElementsByStoreName(storeNameUrl);
        } else if (!storeNameUrl) {
            showAllElements();
        }
    }
    if (storeNameUrl) {
        filterOnPageLoad();
    }


    $("#filterSelect").on('change', function (e) {
        var selectedValue = this.value;
        if (selectedValue == "Price(High>Low)") {
            sortElementsByPriceDescending();
        }
        else if (selectedValue == "Price(Low>High)") {
            sortElementsByPriceascending();
        }
        else if (selectedValue != "filter" && selectedValue != "Price(Low>High)" && selectedValue != "Price(High>Low)" )  {
            event.preventDefault();
            // Get the store name from the data-storename attribute of the button
            var storeNameAttr = $(this).find('option:selected').val();
            console.log(storeNameAttr);
            // Add the storeName as a query parameter to the URL
            var currentUrl = new URL(window.location.href);
            urlParams.set('storeName', storeNameAttr);
            var newQueryString = urlParams.toString();
            var newUrl = currentUrl.origin + currentUrl.pathname + '?' + newQueryString;
            history.pushState({}, '', newUrl);
            // Filter the elements based on the storeName query parameter
            filterElementsByStoreName(storeNameAttr);
            $("#avalabileComponents").text(storeCounts[storeNameAttr]);
        }
    });
    function sortElementsByPriceDescending() {
        var row = $("#componentRow");
        var productCols = row.children('.productCol');
        productCols.sort(function (a, b) {
            var priceA = parseFloat($(a).attr('data-price').replace(/,/g, ''));
            var priceB = parseFloat($(b).attr('data-price').replace(/,/g, ''));
            return priceB - priceA;
        });
        row.empty();
        productCols.appendTo(row);
    }
    $(document).on('click', '.stores', function () {
        event.preventDefault();
        // Get the store name from the data-storename attribute of the button
        var storeNameAttr = $(this).data('storename');
        // Add the storeName as a query parameter to the URL
        var currentUrl = new URL(window.location.href);
        urlParams.set('storeName', storeNameAttr);
        var newQueryString = urlParams.toString();
        var newUrl = currentUrl.origin + currentUrl.pathname +  '?' + newQueryString;
        history.pushState({}, '', newUrl);
        // Filter the elements based on the storeName query parameter
        filterElementsByStoreName(storeNameAttr);
    });
    function sortElementsByPriceascending() {
        var row = $("#componentRow");
        var productCols = row.children('.productCol');
        productCols.sort(function (a, b) {
            var priceA = parseFloat($(a).attr('data-price').replace(/,/g, '')); // remove commas from price with empty string this allow to convert string to number 
            var priceB = parseFloat($(b).attr('data-price').replace(/,/g, ''));
            return priceA - priceB;
        });
        row.empty();
        productCols.appendTo(row);
    }
    window.onpopstate = function (event) {
       // location.reload();
        urlParams = new URLSearchParams(window.location.search);
        storeNameUrl = urlParams.get('storeName');
        if (storeNameUrl) {
            filterElementsByStoreName(storeNameUrl);
        } else if (!storeNameUrl) {
            showAllElements();
            $("#avalabileComponents").text(totalComponentsCounter);
        }
        filterOnPageLoad();
    };
    window.addEventListener('load', function () {
        $(document).ready(function () {
        urlParams = new URLSearchParams(window.location.search);
            storeNameUrl = urlParams.get('storeName');
        if (storeNameUrl) {
            filterElementsByStoreName(storeNameUrl);
        } else {
            showAllElements();
        }
        });

        filterOnPageLoad();
    });
    $(document).on('click', '#compareButton', function () {
        var $detailsButton = $(this).closest('.componentCard').find('#productCard');
        var detailsProductId = $detailsButton.data('id');
        var productSrc = $(this).closest(".componentCard").find(".componentImg img").attr("src");
        var productName = $(this).closest(".componentCard").find("h1").text();
        var productCategory = $detailsButton.data('category');
        var productStoreName = $detailsButton.data('storename');
        var existingProductIds = JSON.parse(localStorage.getItem('selectedProducts')) || [];
        var myData = localStorage.getItem("selectedProducts");
        //if local stoarge is empty give it zero value
        var length = myData ? JSON.parse(myData).length : 0;
        var alreadyExists = existingProductIds.some(function (product) {
            return product.id === detailsProductId;
        });
        if (alreadyExists) {
            // If product already exists in the array, do nothing
            const toast = $('#toast-SSQxZFzM');
            toast.removeClass('hide');
            toast.addClass('show');
            toast.css("opacity", 1);
            $("#alert").text("Product already exists");
            setTimeout(() => {
                toast.addClass('hide');
            }, 4000);
            return;
        }
        if (length < 2) {
           
            existingProductIds.push({
                id: detailsProductId,
                src: productSrc,
                name: productName,
                storeName: productStoreName,
                category: productCategory,
               
            });
           
            const toast = $('#toast-SSQxZFzM');
            toast.removeClass('hide');
            toast.addClass('show');
            toast.css("opacity", 1);
            $("#alert").text("Product added successfully");
            setTimeout(() => {
                toast.addClass('hide');
            }, 4000);
        }
        else if (length == 2) {
            // $("#backgroundWrap").show();
            // $(".productCompareSpecificationsDialog").show();
            const toast = $('#toast-SSQxZFzM');
            toast.removeClass('hide');
            toast.addClass('show');
            toast.css("opacity", 1);
            $("#alert").text("Cant add more product");
            setTimeout(() => {
                toast.addClass('hide');
            }, 4000);

           
            return;
        }
        localStorage.setItem("selectedProducts", JSON.stringify(existingProductIds));
        $(".dropdown-content").empty();
        for (var i = 0; i < existingProductIds.length; i++) {
            $(".dropdown-content").append(`
      <div class="option-container">
        <div class="row">
          <div class="col-5 d-flex">
            <div class="option-img d-flex justify-content-center">
              <img src=${existingProductIds[i].src} />
            </div>
          </div>
          <div class="col-5 d-flex">
            <div class="option-title d-flex justify-content-center">
              <h1 class="option-title">${existingProductIds[i].name}</h1>
            </div>
          </div>
          <div class="col-2 d-flex justify-content-between">
            <div class="removeProduct d-flex justify-content-center">
              <button onclick="removeProductFromComparimation()" data-id=${existingProductIds[i].id} class="removeProductFromComparimation" id="removeProduct">X</button>
            </div>
          </div>
        </div>
      </div>
    `);
        }
        var counter = length + 1;
        const selectProductLink = $('.selectProduct');
        if (!$('.productsComparison').length) {
            const numberSpan = $('<span class="productsComparison">' + counter + '</span>');
            selectProductLink.after(numberSpan);
            numberSpan.show();
            // Element exists
        } else {
            $(".productsComparison").text(counter);
        }
        var dropdownContentChildrens = $(".dropdown-content").children('.option-container').length;
        if (dropdownContentChildrens > 1) {

            $(".dropdown-content").append(`<div onClick="openComparisonPage()" class="openProductsComparisonPage"> <a>Comparison page</a> </div>`);

        }

      
    });
    function filterElementsByStoreName(storeName) {

        var row = $("#componentRow");
        var productCols = row.children('.productCol');
        var clickedStoreComponentsNumber = 0;
        productCols.each(function () {
            if ($(this).attr('data-storeName') != storeName) {
                $(this).hide();
            } else {
                clickedStoreComponentsNumber = 1 + clickedStoreComponentsNumber;
                $("#avalabileComponents").text(clickedStoreComponentsNumber);
                $(this).show();
            }

        });
    }
    $(document).on('click', '#productCard', function () {
        $("#backgroundWrap").css("overflow", "hidden");
        var productId = $(this).data("id");
        var  productStoreName = $(this).data("storename");
        var productCategory = $(this).data("category");
        $(".dropdown-container").css("position", "inherit");
        loadProductDeatils(productCategory, productId);
    });
    $(document).on('click', '#productLink', function () {
        var link = $(this).attr("data-link");
        // Create a temporary input element to copy the link to the clipboard
        var tempInput = $("<input>");
        $("body").append(tempInput);
        tempInput.val(link).select();
        document.execCommand("copy");
        tempInput.remove();
        const toast = $('#toast-SSQxZFzM');
        toast.removeClass('hide');
        toast.addClass('show');
        toast.css("opacity", 1);
        $("#alert").text("Product link copied");
        setTimeout(() => {
            toast.addClass('hide');
        }, 4000);

    });
    $("#exitDialog").click(function () {
        $(".productSpecificationsDialog").hide();
        $("#backgroundWrap").hide();
        $(".dropdown-container").css("position", "relative");
        $("#backgroundWrap").css("overflow", "auto");
    });
    $("#exitComparison").click(function () {
        $(".productsSpecifications").children().remove();
        $(".productSpecifications .imgRow").children().remove();
        localStorage.removeItem("selectedProducts");
        $(".productCompareSpecificationsDialog").hide();
        $("#backgroundWrap").hide();
        $("#compareDialogDescreptionUl").remove();
        $(".dropdown-container").css("position", "relative");
        $(".dropdown-content").children().remove();
        var dropdownContentChildrens = $(".dropdown-content").children('.option-container').length;
        $(".productsComparison").text(dropdownContentChildrens);
        $(".productsComparison").remove();

    });

dropdownBtn.on('click', function () {
    dropdownContent.toggleClass('show');
});

});
function toggleDropdown() {
    var existingProductIds = JSON.parse(localStorage.getItem('selectedProducts')) || [];
    var dropdown = document.getElementById("product-dropdown");
    if (dropdown.style.display === "block") {
        dropdown.style.display = "none";

    } else if (dropdown.style.display === "none" && existingProductIds.length > 0) {
        dropdown.style.display = "block";
        // Log the IDs to the console
        // console.log(ids);   
    } else if (existingProductIds.length === 0) {
        const toast = $('#toast-SSQxZFzM');
        toast.removeClass('hide');
        toast.addClass('show');
        toast.css("opacity", 1);
        $("#alert").text("No such items in compare list");
        setTimeout(() => {
            toast.addClass('hide');
        }, 4000);

    } else {
        dropdown.style.display = "block";


        // Log the IDs to the console
        // console.log(ids);   
    }
}

function openComparisonPage() {
    var existingProductIds = JSON.parse(localStorage.getItem('selectedProducts')) || [];
    var dropdown = document.getElementById("product-dropdown");
    $("#backgroundWrap").show();
    $(".productCompareSpecificationsDialog").show();
    $(".dropdown-container").css("position", "inherit");
    dropdown.style.display = "none";
    for (var i = 0; i < existingProductIds.length; i++) {
        var productId = existingProductIds[i].id;
        var storeName = existingProductIds[i].storeName;
        var categoryName = existingProductIds[i].category;
        loadpRroductCompareDeatils(categoryName, productId)
    }
}
function showAllElements() {
    var row = $("#componentRow");
    var productCols = row.children('.productCol');
    productCols.show();
}
function removeProductFromComparimation() {
    var existingProductIds = JSON.parse(localStorage.getItem('selectedProducts')) || [];
    // Add event listener to dropdown-content container
    $('.dropdown-content').on('click', '.removeProductFromComparimation', function () {
        // Get the data-id attribute of the clicked button
        var productId = $(this).data('id');

        // Find the index of the product in existingProductIds
        var index = existingProductIds.findIndex(function (product) {
            return product.id === productId;
        });

        // Remove the product from the array
        existingProductIds.splice(index, 1);

        // Update the local storage
        localStorage.setItem('selectedProducts', JSON.stringify(existingProductIds));

        // Remove the product from the dropdown content
        $(this).closest('.option-container').remove();
        var dropdown = document.getElementById("product-dropdown");
        var dropdownContentChildrens = $(".dropdown-content").children('.option-container').length;
        if (dropdownContentChildrens === 0) {
            dropdown.style.display = "none";
            $(".productsComparison").remove();
        }

        var ProductsComparison = $(".openProductsComparisonPage");
        console.log(dropdownContentChildrens);
        if (dropdownContentChildrens != 2 && ProductsComparison.length) {
            ProductsComparison.remove();
        }
        $(".productsComparison").text(dropdownContentChildrens);

       

    });
}
function loadItems() {
    
    $.ajax({
        url: '/api/products/' + category,
        method: 'GET',
        success: function (data) {
            data.forEach(function (component) {
                ++totalComponentsCounter;
                if (storeCounts[component.storeName]) {
                    storeCounts[component.storeName]++;
                } else {
                    storeCounts[component.storeName] = 1;
                }
                storeCounts["s"] = 1;
                console.log(storeCounts["s"]);
                if (!storesName.has(component.storeName)) {
                    storesName.add(component.storeName);
               
                }
                $("#avalabileComponents").text(totalComponentsCounter);
                $("#componentRow").append("<div class='productCol col-lg-2 col-md-4 col-sm-12 m-3' data-storename=" + component.storeName + " data-price=" + component.price + ">  <div class='componentCard h-100 d-flex flex-column product' > <div class='deatils' id='productCard' data-storename=" + component.storeName + " data-id=" + component.productId + " data-category=" + component.category + " > <div class='componentImg'> <img src='" + component.image + " ' class='img-fluid '></div><h1 class='text-center'>" + component.name + "</h1><div class='prices d-flex justify-content-center'><del class='oldPrice' id='oldPrice'>" + component.oldPrice + "</del><span class='price'>" + component.price + "JOD</span></div><div class='storeName'><span>" + component.storeName + "</span></div></div><div class='compareButton'><a class='compare' id='compareButton' data-id=" + component.productId + "  data-category=" + component.category + " data-storename=" + component.storeName + ">Compare</a></div>  </div> </div></div>");
            });
            for (let value of storesName) {
                console.log(value);
                $("#filterSelect").append("<option value='" + value + "'>" + value +"</option>");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('Error: ' + textStatus + ' - ' + errorThrown);
        }
    });
   
}
function loadProductDeatils(category, productId) {
        $.ajax({
            url: '/api/products/' + category + '/' + productId,
            method: 'GET',
            success: function (data) {
                data.forEach(function (component) {
                    var name = component.name;
                    var oldPrice = component.oldPrice;
                    var price = component.price;
                    var brand = component.brand;
                    var img = component.image;
                    const description = component.description;
                    var date = component.date;
                    var time = component.time;
                    var productUrl = component.productUrl;
                    $(".desciptionListItem").remove();
                    $('#dialogImg').attr('src', img);
                    $("#dialogName").text(name);
                    $("#dialogBrand").text("Brand : " + brand);
                    $("#dialogPrice").text("Price : " + price + "");
                    $("#dialogDateTime").text("Last update : " + date + ":" + time);
                    for (desc in description) {
                        $("#dialogDescreptionUl").append('<li class="desciptionListItem">' + description[desc] + '</li>');
                    }
                    if (oldPrice != "0") {

                        $(".productPrice").append('<del id="dialogOldPrice"> </del>');
                        $("#dialogOldPrice").text(oldPrice);
                        $("#dialogOldPrice").show
                    }
                    else {
                        $("#dialogOldPrice").remove();
                    }
                    if (name != null) {
                        $("#backgroundWrap").show();
                        $(".productSpecificationsDialog").show();
                    }
                    else {
                        const toast = $('#toast-SSQxZFzM');
                        toast.removeClass('hide');
                        toast.addClass('show');
                        toast.css("opacity", 1);
                        $("#alert").text("Error loading detail");
                        setTimeout(() => {
                            toast.addClass('hide');
                        }, 4000);

                    }
                    if (productUrl != null) {
                        $("#productLink").attr("data-link", productUrl);
                    }

                });


                console.log(data);
                // Do something with the response data
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('Error: ' + textStatus + ' - ' + errorThrown);
            }
        });
     
} 
function loadpRroductCompareDeatils(category, productId) {
    $.ajax({
        url: '/api/products/' + category + '/' + productId,
        method: 'GET',
        success: function (data) {
            data.forEach(function (component) {
                $(".productSpecifications .imgRow").append(`<div class="col-lg-5 justify-content-center d-flex col-md-5 col-sm-12">
                                                 <div class="proudctImg align-self-center ">
                                                  <img  src=`+ component.image + `></div></div>`);
                $(".productsSpecifications").append(`<div class="col-lg-6 p-0 col-md-7 col-sm-12 ">
                                        <div class="productCompareSpecifications d-flex justify-content-center  h-100 ">
                                            <div class="align-self-center w-100 ">
                                                <div class="productName compareSpecification">
                                                    <h2 >`+ component.name + `</h2>
                                                </div>
                                                 <div class="productStore compareSpecification">
                                                    <h2 >`+ component.storeName + `</h2>
                                                </div>
                                                <div class="productBrand compareSpecification">
                                                    <h2 >`+ component.brand + `</h2>
                                                </div>
                                                <div class="productPrice d-flex compareSpecification">
                                                    <h2>`+ component.price + ` </h2>
                                                    <del class="compareOldPrice">` + component.oldPrice + ` </del>
                                                </div>
                                                <div class="compareDescription  d-grid">
                                                    <div class="compareDescriptionDropDownContent" >
                                                        <ul class="compareDialogDescreptionUl" id=`+ productId + `>
                                                        </ul>
                                                    </div>
                                                </div>
                                                <div class="productButtons d-flex justify-content-center">
                                                    <a class="productButton" id="productLink" data-link=`+ component.productUrl + `>Product link</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>`);
                if (component.oldPrice != "0") {
                    $(".compareSpecification del").show();
                } else {
                    $(".compareSpecification del").hide();
                }
                var description = component.description;
                for (desc in description) {
                    $("#" + productId).append('<li class="desciptionListItem">' + description[desc] + '</li>')
                }

            });


            console.log(data);
            // Do something with the response data
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('Error: ' + textStatus + ' - ' + errorThrown);
        }
    });
}
function loadItemsBySearch(Name) {
    $.ajax({
        url: '/api/products/name/' + Name,
        method: 'GET',
        success: function (data) {
            totalComponentsCounter
            if (data.length > 0) {
                data.forEach(function (component) {
                    // Increment the count for the current store
                    if (storeCounts[component.storeName]) {
                        storeCounts[component.storeName]++;
                    } else {
                        storeCounts[component.storeName] = 1;
                    }
                    ++totalComponentsCounter;
                    if (!storesName.has(component.storeName)) {
                        storesName.add(component.storeName);
                    }
                    $("#avalabileComponents").text(totalComponentsCounter);
                    $("#componentRow").append("<div class='productCol col-lg-2 col-md-4 col-sm-12 m-3' data-storename=" + component.storeName + " data-price=" + component.price + ">  <div class='componentCard h-100 d-flex flex-column product' > <div class='deatils' id='productCard' data-storename=" + component.storeName + " data-id=" + component.productId + " data-category=" + component.category + " > <div class='componentImg'> <img src='" + component.image + " ' class='img-fluid '></div><h1 class='text-center'>" + component.name + "</h1><div class='prices d-flex justify-content-center'><del class='oldPrice' id='oldPrice'>" + component.oldPrice + "</del><span class='price'>" + component.price + "JOD</span></div><div class='storeName'><span>" + component.storeName + "</span></div></div><div class='compareButton'><a class='compare' id='compareButton' data-id=" + component.productId + "  data-category=" + component.category + " data-storename=" + component.storeName + ">Compare</a></div>  </div> </div></div>");
                });
                for (const value of storesName) {
                    $("#filterSelect").append("<option value='" + value + "'>" + value + "</option>");
                }
            } 
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 404) {
                console.log('Not found');

            }

        }
    });
  
}
function filterOnPageLoad() {
    if (storeNameUrl) {
        // Get the value you want to select
        var valueToSelect = storeNameUrl; 
        $("#avalabileComponents").text(storeCounts[valueToSelect]);
        // Set the selected attribute for the option with the valueToSelect
        $('#filterSelect').find('option[value="' + valueToSelect + '"]').prop('selected', true);
    }
    else {
        // Get the value you want to select
        var valueToSelect = "Filter";
        $("#avalabileComponents").text(storeCounts[totalComponentsCounter]);
        // Set the selected attribute for the option with the valueToSelect
        $('#filterSelect').find('option[value="' + valueToSelect + '"]').prop('selected', true);
    }
}