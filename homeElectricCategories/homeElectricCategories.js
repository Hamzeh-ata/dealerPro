
$(document).ready(function () {

    var categoriesDesciptions = {
        "Tv's": "Electronic devices that receive and display broadcast signals for entertainment and information purposes.",
        "Dishwashers": "Appliances that clean and sanitize dishes by using water and detergent, often saving time and effort compared to handwashing.",
        "refrigerators": "Refrigeration units used to store and preserve food and beverages, keeping them at low temperatures to slow down bacterial growth and maintain freshness.",
        "washingMachines": "Machines that clean clothes and other fabrics by using water, detergent, and mechanical action to remove dirt and stains, often accompanied by spin cycles to remove excess water.",
    };
    var productNames = {
        "Tv's": "Televisions",
        "Dishwashers": "Dish washers",
        "refrigerators": "Fridges",
        "washingMachines": "Washers",
    }


    $(".categorieCard").click(function () {

        //change mainImg based on clicked card id
        var cardId = $(this).attr("id");
        $(".mainImg img").attr("src", "./imgs/" + cardId + ".png");
        //change name text based on clicked card
        var productName = productNames[cardId];
        $(".mainText h1").text(productName);
        //change description text based on clicked card
        var descriptionText = categoriesDesciptions[cardId];
        $(".mainText p").text(descriptionText);
        $(".getStarted").css("display", "flex");
        $(".getStarted a").attr("href", 'products?category=' + encodeURIComponent(cardId));
       // window.location.href = 'products?category=' + encodeURIComponent(productName); // Redirect to product page with search parameter in URL
        
    });

});