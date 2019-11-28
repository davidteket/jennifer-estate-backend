window.onload = function () {
    $("#menu-strip").load("site/shared/menu_strip.html");
    $("#image-slider").load("site/shared/image_slider.html");
    $("#search-module").load("site/shared/search_module.html");
    $("#advanced-search-container").append($("<div>").load("site/shared/advanced_search.html"));
    $("#advanced-search-container").hide(0);
};

// Eseménykezelés ----------------------------------------------------------------------------------------------------

$(document).on('click', '#advanced-search', function() {
    $("#advanced-search-container").show(1000);
    $("#advanced-search").hide(1000);
    $("#search-module").hide(1000);
});

// Sablonkezelés ----------------------------------------------------------------------------------------------------

// main
//
$(document).on('click', '#homepage-btn', function() {

    $("#contents").empty();

    $("#contents").append($("<div>").load("site/main.html", null, function() {
        $("#menu-strip").load("site/shared/menu_strip.html");
        $("#image-slider").load("site/shared/image_slider.html");
        $("#search-module").load("site/shared/search_module.html");
        $("#advanced-search-container").append($("<div>").load("site/shared/advanced_search.html"));
        $("#advanced-search-container").hide(0);
    }));

});

// estates
//
$(document).on('click', '#estates-btn', function() {

    $("#contents").empty();

    $("#contents").append($("<div>").load("site/estates.html", null, function() {

        $("#menu-strip").load("site/shared/menu_strip.html");
        $("#search-module").load("site/shared/search_module.html");

        $("#advanced-search-container").append($("<div>").load("site/shared/advanced_search.html"));
        $("#advanced-search-container").hide(0);

        $("#estate-lister").load("site/shared/estate_lister.html", null, function() {

            // ingatlanok lekérése és listázása:
            //
            $.getJSON("/Estate/GetEstates", null, function(result) {
                
                var row = $("#estates").append($("<div class='row'>"));
                for(var i = 0; i < result.length; ++i) 
                {
                    row.append($("<div name='estate' id="+ result[i].EstateId +" class='col-sm-4'>")
                                 .load("site/shared/estate_card.html", null, function() {
                                     
                                        // TODO: be kell helyettesíteni a kapott adatpontokat a kártyába.
                    }));

                    if (i > 0 && i % 3 == 0) 
                        row = $("#estates").append($("<div class='row'>"));
                } 

            });

        });
    }));

});

// customer_request
//
$(document).on('click', '#custreq-btn', function() {

    // TODO

});

// employees
//
$(document).on('click', '#employees-btn', function() {

    // TODO

});

// contact
//
$(document).on('click', '#contact-btn', function() {

    // TODO

});

// about
//
$(document).on('click', '#about-btn', function() {

    // TODO

});