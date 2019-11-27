window.onload = function() {

    $("#advanced-search").click(function() 
    {
        $("#advanced-search-container").load("site/shared/advanced_search.html").show(1000);
        $("#search-buttons").hide(1000);
        return false;
    });

    $("#start-simple-search").click(function()
    {
        alert("indítsd el az egyszerű keresést");
        return false;
    });

};