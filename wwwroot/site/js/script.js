// Mindenkori oldalbetöltés.
//
window.onload = function () {

    $("#menu-strip").load("site/shared/menu_strip.html");

    // Dinamikus.
    //
    if ($("#image-slider").length)
        $("#image-slider").load("site/shared/image_slider.html", null, function() {

            $.get('/Estate/RandomShowcase', null, function(response) {

                var images = JSON.parse(response);
                var url = '/static/';

                if (images)
                {
                    $("#img-slider-first").attr('src', url + images[0]);
                    $("#img-slider-second").attr('src', url + images[1]);
                    $("#img-slider-third").attr('src', url + images[2]);
                    $('.carousel').carousel();
                }

            });

        });

    if ($("#estate-profile-container").length)
        $("#estate-profile-container").load("site/shared/estate_profile.html", function() {
          
            var id = getUrlVars()["estateId"];

            if (id != null) {
                $.getJSON('/Estate/Detail?estateId=' + id, function(response) {

                    var imgRequestPath = '/static/';
                    if (response.Images.length != 0) {

                        // Elsődleges kép:
                        //
                        $("#estate-profile-img-primary").attr('src', imgRequestPath + response.Images[0].Id + response.Images[0].Extension);

                        // További képek:
                        //
                        for (var row = 0, j = 1; j < response.Images.length; ++j) 
                        {
                            if (row % 3 == 0)
                                $("#additional-estate-pictures").append($("<div class='row' id='row-" + row + "'>"));

                            if (j < 4) 
                            {
                                $("#row-" + row).append($("<div class='col' id='img-col-" + j + "'>"));
                                $("#img-col-" + j)
                                .append($("<img src='" + imgRequestPath + response.Images[j].Id + response.Images[j].Extension + "' class='img-fluid site-link-pointer rounded float-left'  style='max-height: 220px;'>"));
                            }
                            else
                                break;
                        }

                        if (response.Images.length > 4)
                            $("#additional-picture-count").text("Még " + (response.Images.length - 4).toString() + " kép");
                        else
                            $("#additional-picture-count").hide();
                    }
                    else {
                        $("#additional-picture-count").hide();
                    }
                    
                    $("#ad-title").append(response.Advertisement.Title);
                    $("#ad-location").append(response.Address.PostCode + ', ' +
                                            response.Address.Street + ', ' +
                                            response.Address.City + ', ' +
                                            response.Address.County + ', ' +
                                            response.Address.Country);
                    $("#ad-price").append(currencyFormatter.format(response.Estate.Price));
                    $("#ad-description").append(response.Advertisement.DescriptionDetail);
                    $('#estate-detailsheet').hide();

                    // OpenStreetMap lekérdezés és térképbeállítás.
                    //
                    var osmQuery = "https://nominatim.openstreetmap.org/search?";

                    var location = "street=" + response.Address.HouseNumber + " " + response.Address.Street + "&" +
                                   "city=" + response.Address.City + "&" +
                                   "county=" + response.Address.County + "&" +
                                   "country=" + response.Address.Country + "&" +
                                   "postalcode=" + response.Address.PostCode + "&format=json";

                    osmQuery += location;
                    $.getJSON(osmQuery, null, function(response) {

                        if (response.length == 0) 
                            $('#openstreetmap-embedded').hide();
                        else {
                            var map = 'https://www.openstreetmap.org/export/embed.html?';

                            var x1 = response[0]["boundingbox"][2];
                            var x2 = response[0]["boundingbox"][3];
                            var y1 = response[0]["boundingbox"][0];
                            var y2 = response[0]["boundingbox"][1];

                            var space = '%2C';
                            var amp = '&amp;';

                            var boundingBox = 'bbox=' + x1 + space + y1  + space + x2 + space + y2;
                            var marker = 'marker=' + response[0]["lat"] + space + response[0]["lon"];

                            map += boundingBox + amp + marker + "&layers=ND";

                            $('#openstreetmap-embedded').attr('src', map);
                        }

                    });
                    
                    //$("#openstreetmap-embedded").attr('src', osmEmbed);

                    // Authorizált.
                    //
                    if (response.Images.length < 4 || response.Images.length == 0) {
                        $.getJSON('/Employee/GetCurrentUserId', null, function(data) {
                            if (data.ItemId != null) {
                                $("#alert-for-logged-in-users").show(500);
                            }
                        });
                    }
                });
            }
            
        });

    $("#search-module").load("site/shared/search_module.html", function() {
            $('#search-module-container').hide();

        $("#add-new-estate").hide();   
    });

    $("#advanced-search-container").append($("<div>").load("site/shared/advanced_search.html"));
    $("#advanced-search-container").hide(0);


    if ($("#sorter-strip").length)
        $("#sorter-strip").append($("<div>").load('site/shared/sorter_strip.html', null, function() {

            // Alapértelmezetten a legfrissebb feltöltéseket mutatja.
            //
            $("#expensive-first-btn").hide();
            $("#oldest-first-btn").hide();

        }));

    if ($("#estate-lister").length)
        loadEstates();

    $("#footer-strip").load("site/shared/footer_strip.html", null, function() {
        $("#invalid-credentials-login").hide();
        $("#waiting-for-authorization").hide();
        $("#valid-credentials-login").hide();
    });

    // Authorizált.
    //
    $.getJSON('/Employee/GetCurrentUserId', null, function(data) {

        if (data.ItemId != null) 
        {
            $("#add-new-estate").show();
            $("#login-modal-popup").empty().replaceWith("<h5 id='logout-btn' class='small btn btn-link col site-link-pointer pb-2 pt-2'><span class='fas fa-sign-out-alt'></span></span>&nbsp;&nbsp;Kijelentkezés</h5>");
            $("#alert-for-logged-in-users").hide();
            $("#upload-estate-images-btn").show();
            $("#edit-estate-btn").show();
            $("#delete-estate-btn").show();

            $("#admin-text").empty();
            $("#admin-text").append("<span id='admin-logo' class='fas fa-cog'></span>&nbsp;&nbsp;Admin Felület");
            $("#admin-text").attr('href', '/admin.html');

            // Admin felület.
            //
            if($('.admin').length) {
                $.getJSON('/Employee/Details?employeeId=' + data.ItemId, function(response) {

                    if (response.EmployeeId == data.ItemId)
                    {
                        /*
                        ProfilePictureId: "default/no-image.jpg"    <-- TODO
                        */

                        $('#logged-in-user-fullname').text(response.LastName + " " + response.FirstName);
                        $('#user-profile-username').text(response.UserName);
                        $('#user-profile-adcount').text(response.AdvertisementCount);
                        var roles = "";
                        for (var i = 0; i < response.EmployeeRoles.length; ++i) {
                            if ((i + 1) < response.EmployeeRoles.length)
                                roles += response.EmployeeRoles[i] + ", ";
                            else 
                                roles += response.EmployeeRoles[i] + ".";
                        }
                        $('#user-profile-roles').text(roles);
                        $('#user-profile-email').text(response.Email);
                        $('#user-profile-phone').text(response.Phone == null ? "Nincs megadva." : response.Phone);
                        $('#user-profile-description').text(response.Description == null ? "Nincs megadva." : response.Description);
                        $('#user-profile-profilepicture').attr('src', '/static/' + response.ProfilePictureId);

                        $('#dbase-erease-inprogress').hide();
                        $('#dbase-erease-error').hide();
                        $('#dbase-erease-success').hide();
                        $('#role-added-message').hide();
                    }
                    else {
                        $('#add-new-role-btn').hide();
                        $('#erease-dbase-btn').hide();
                    }

                });
            }
        }
        else {
            $("#alert-for-logged-in-users").hide();
            $("#upload-estate-images-btn").hide();
            $("#edit-estate-btn").hide();
            $("#delete-estate-btn").hide();
            $("#delete-gallery-item").hide();
        }
    });
};

// Árformátum.
//
var currencyFormatter = new Intl.NumberFormat('hu-HU', {
    style: 'currency',
    currency: 'HUF',
    minimumFractionDigits: 0
});

// Query-string szerializálás.
//
function getUrlVars() {

    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }

    return vars;
}

// Drag n drop állapotmentesítés eseménynél.
//
window.addEventListener("dragover", function(e) {
    
    e = e || event;
    e.preventDefault();

}, false);

window.addEventListener("drop", function(e) {

    e = e || event;
    e.preventDefault();

}, false);

function redirectToEstateProfilePage(url) 
{
    window.location.href = url;
}

// Hirdetések listázása.
//
function loadEstates() {

    $("#contents").append($("<div>").load("/estates.html", null, function() {

        if ($(".home").length)
            $("#search-btn-container").hide();

        $("#estate-lister").load("site/shared/estate_lister.html", null, function() {

            // Ingatlanok lekérése és listázása:
            //
            $.getJSON("/Estate/Load", null, function(estates) {

                if (getUrlVars()["orderBy"] == "expensiveFirst") {
                    estates = highestPrice(estates);
                    expensive();
                }
                if (getUrlVars()["orderBy"] == "cheapestFirst") {
                    estates = lowestPrice(estates);
                    cheapest();
                }
                if (getUrlVars()["orderBy"] == "oldestFirst") {
                    estates = newestDate(estates);
                    oldest();
                }
                if (getUrlVars()["orderBy"] == "newestFirst") {
                    estates = oldestDate(estates);
                    newest();
                }

                var row = $("#estates");
                if (estates.length == 0) {
                    $("#no-estates-to-display").show(1000);
                    return;
                }
                else 
                    $("#no-estates-to-display").hide();

                for(var i = 0; i < estates.length; ++i) 
                {
                    if ($('.home').length && i == 4)
                        break;

                    var j = 0;
                    var url = '/estate-details.html' + '?' + 'estateId=' + estates[i].EstateId;

                    row.append($("<div id=" + estates[i].EstateId + " class='col-sm-3 float-left' onclick='redirectToEstateProfilePage(\"" + url + "\")'>").load("site/shared/estate_card.html", null, function(result) {

                            var cardId = '#' + estates[j].EstateId;

                            $(cardId).find(".card-img-top").attr('src', '/static/' + estates[j].Image);
                            $(cardId).find(".estate-card-offertype").append(estates[j].OfferType);
                            $(cardId).find(".estate-card-title").append(estates[j].Title);
                            $(cardId).find(".estate-card-price").append(currencyFormatter.format(estates[j].Price));

                            var basicInfo = estates[j].PostCode + ', ' +
                                            estates[j].City  + ', ' +
                                            estates[j].Street + ', ' +
                                            estates[j].Country;
                                            
                            if (basicInfo.length >= 25)
                                basicInfo = basicInfo.substr(0, 25) + '...';

                            $(cardId).find(".estate-card-basic-info").append(basicInfo);


                            ++j;
                                        
                    }));
                } 

            });

        });
    }));
}

// Összetett kereső.
//
$(document).on('click', '#advanced-search', function() {

    $("#advanced-search-container").show(1000);
    $("#search-module").hide(500);

});

// Vissza az egyszerű keresőhöz.
//
$(document).on('click', '#back-to-simple-search-btn', function() {

    $("#search-module").show(1000);
    $("#advanced-search-container").hide(500);

});

// Tag klikkelve.
//
$('.tag-elem').click(function() {

    var toggler = +$(this).val();
    
    if (toggler == 1)
        $(this).attr('value', false);
    else 
        $(this).attr('value', true);

    $(this).toggleClass('tag-clicked');
    return false;
})

// Bejelentkezés.
//
$(document).on('click', '#login-btn', function() {

    $("#invalid-credentials-login").hide(500);
    $("#waiting-for-authorization").show(1000);

    var username = $("#username").val();
    var password = $("#password").val();

    var loginObject = {

        UserName: username,
        Password: password
    };

    var body = JSON.stringify(loginObject);

    $.post('/Employee/Login', body, function(response) {

        var result = JSON.parse(response);

        if (result.Success == true) {

            $("#waiting-for-authorization").hide(500);
            $("#valid-credentials-login").show(1000);

            $("#add-new-estate").show();

            window.setTimeout(function() {
                window.location.href = '/admin.html';
            }, 2000);
        }
        else {

            $("#waiting-for-authorization").hide(500);
            $("#invalid-credentials-login").show(1000);

        }
    });

});

// Kijelentkezés.
//
$(document).on('click', '#logout-btn', function() {

    $.post('/Employee/Logout', null, function(response) {

        var result = JSON.parse(response);
        alert(result.Message);
        window.location.href = '/home.html';

    });

});

// Hirdetés feltöltés.
//
$(document).on('click', '#upload-estate-btn', function() {

    var creationTime = new Date($.now());

    $.get('/Employee/GetCurrentUserId', null, function(response) {

        var result = JSON.parse(response);

        if (result.Success == true) {

            var advertiserId = result.ItemId;

            var estateObject = {

                Estate : {

                    Id : 0,
                    Price : +$("#new-estate-price").val(),
                    TotalSquareFeet : +$("#new-estate-squarefeet").val(),
                    LandSquareFeet : +$("#new-estate-propertysquarefeet").val(),
                    RoomCount : +$("#new-estate-roomcount").val(),
                    KitchenCount : +$("#new-estate-kitchencount").val(),
                    BathroomCount : +$("#new-estate-bathroomcount").val(),
                    FloorCount : +$("#new-estate-floorcount").val(),
                    GarageSquareFeet : +$("#new-estate-garagesquarefeet").val(),
                    GardenSquareFeet : +$("#new-estate-gardensquarefeet").val(),
                    TerraceSquareFeet : +$("#new-estate-terracesquarefeet").val(),
                    
                    BuiltAt : $("#new-estate-builtat").val() + '-01-01',
                    RefurbishedAt : $("#new-estate-refurbishedat").val() + '-01-01',    // TODO

                    HasElevator : $("#new-estate-haselevator").attr('value'),
                    HasGarden : $("#new-estate-hasgarden").attr('value'),
                    HasGarage : $("#new-estate-hasgarage").attr('value'),
                    HasDisabledFriendly : $("#new-estate-disabilityfriendly").attr('value'),
                    HasInnerHeightGreatherThan3Meters : $("#new-estate-innerheightgreaterthan3meters").attr('value'),
                    HasSeparateWcAndBathroom : $("#new-estate-hasseparatebathandtoilet").attr('value'),
                    HasParticipatedInThePanelProgram : $("#new-estate-haspanelprogramparticipation").attr('value'),
                    HasHeatIsolated : $("#new-estate-hasheatisolation").attr('value'),
                    HasTerace : $("#new-estate-hasterrace").attr('value'),
                    HasBasement : $("#new-estate-hasbasement").attr('value'),

                    Roof : $("#new-estate-roof").val(),
                    Comfort : $("#new-estate-comfort").val(),
                    Outlook : $("#new-estate-view").val(),
                    Area : $("#new-estate-area").val(),
                    Quality : $("#new-estate-quality").val(),
                    Category : $("#new-estate-category").val(),
                    AdvertiserId : advertiserId
                },
        
                Address : {

                    Id : 0,
                    FloorNumber : $("#new-estate-floorcount").val(),
                    EstateId : 0,
                    
                    Country : $("#new-estate-country").val(),
                    County : $("#new-estate-county").val(),
                    City : $("#new-estate-city").val(),
                    PostCode : $("#new-estate-postcode").val(),
                    Street : $("#new-estate-street").val(),
                    HouseNumber : $("#new-estate-housenumber").val(),
                    Door : $("#new-estate-door").val()
                },
        
                Electricity : {

                    Id : 0,
                    EstateId : 0,
                    
                    SunCollector : $("#new-estate-hassuncollector").attr('value'),
                    PowerWall : $("#new-estate-haspowerwall").attr('value'),
                    Networked : $("#new-estate-networkedelectricity").attr('value'),
                },
        
                Heating : {

                    Id : 0,       
                    EstateId : 0,

                    ByCirculation : $("#new-estate-hascirculation").attr('value'),
                    ByGasConvector : $("#new-estate-hasconvector").attr('value'),
                    ByGas : $("#new-estate-hasgasheating").attr('value'),
                    ByWood : $("#new-estate-haswoodenheating").attr('value'),
                    ByCombined : $("#new-estate-hascombinedheating").attr('value'),
                    ByChimney : $("#new-estate-haschimneyheating").attr('value'),
                    ByCockle : $("#new-estate-hascockleheating").attr('value'),
                    ByRemote : $("#new-estate-hasremoteheating").attr('value'),
                    ByNetworked : $("#new-estate-hasgasremote").attr('value'),
                    ByElectricity : $("#new-estate-haselectricityheating").attr('value'),
                    ByFloorHeating : $("#new-estate-hasfloorheating").attr('value')
                    
                },
        
                PublicService : {

                    Id : 0,
                    EstateId : 0,
                    
                    HasGroceryNearby : $("#new-estate-hasgrocerynearby").attr('value'),
                    HasGasStationNearby : $("#new-estate-haspetrolstationnearby").attr('value'),
                    HasTransportNearby : $("#new-estate-haspublictransportnearby").attr('value'),
                    HasDrugStoreNearby : $("#new-estate-hashaspharmacynearby").attr('value'),
                    HasSchoolNearby : $("#new-estate-hasschoolnearby").attr('value'),
                    HasMailDepotNearby : $("#new-estate-hasmaildepotnearby").attr('value'),
                    HasBankNearby : $("#new-estate-hasbanknearby").attr('value'),
                    HasEntertainmentServicesNearby : $("#new-estate-hasentertainmentnearby").attr('value')
                },
        
                Advertisement : {

                    Id : 0,
                    OrderOfAppearance : +$('#new-estate-priority').val(),
                    EstateId : 0,
                    
                    TimePosted : creationTime.getFullYear() + '-' + creationTime.getMonth() + '-' + creationTime.getDay(),
                    LastModification : null,

                    Title : $("#new-estate-adtitle").val(),
                    DescriptionDetail : $("#new-estate-addescr").val(),
                    OfferType : $("#new-estate-adtype").val(),
                    AdvertiserId : advertiserId
                } 
            };
        
            var body = JSON.stringify(estateObject);

            $.post('/Estate/Upload', body, function(response) {

                var result = JSON.parse(response);

                if (result.Success == true) {
                    window.location.href = '/estate-details.html?estateId=' + result.ItemId;
                }
                else
                    alert('feltöltés sikertelen');
            });
        }
    });

    

});

// Hirdetés feltöltés megszakítása.
//
$(document).on('click', '#cancel-upload-estate-btn', function() {

    $('#close-upload-dialog-btn').click();

});

// Hirdetés törés megszakítása.
//
$(document).on('click', '#cancel-estate-delete-btn', function() {

    $('#close-estate-delete-btn').click();

});

// Hirdetés törlés jóváhagyása.
//
$(document).on('click', '#confirm-estate-delete-btn', function() {

    var estateId = getUrlVars()["estateId"];

    $.post('/Estate/Delete?estateId=' + estateId, null, function(response) {

        var result = JSON.parse(response);

        if (result.ItemId != null) {
            window.location.href = '/estates.html';
        }

    });

});

// Drag and drop zóna stílusváltoztatás.
//
$(document).on('dragenter', '#image-upload-dropzone', function(elem) {
    
    
    var item = $(elem.target);
    item.addClass("site-dropactive-zone");

});

$(document).on('dragexit', '#image-upload-dropzone', function(elem) {
    
    var item = $(elem.target);
    item.removeClass("site-dropactive-zone");

});

$(document).on('dragleave', '#image-upload-dropzone', function(elem) {
    
    var item = $(elem.target);
    item.removeClass("site-dropactive-zone");

});

// Képfeltöltés.
//
$(document).on('drop', '#image-upload-dropzone', function(elem) {

    var item = $(elem.target);
    item.removeClass("site-dropactive-zone");

    // Fájlok inputhoz adása.
    //
    $("#image-input-elem").prop("files", elem.originalEvent.dataTransfer.files);

    // Fájlok listázása a feltöltés alatt lévők sorában.
    //
    for (var i = 0; i < $("#image-input-elem")[0].files.length; ++i) 
    {        
        $("#awaiting-for-upload-images-container")
            .append($("<div id=" + $("#image-input-elem")[0].files[i].name.replace('.', '-')  + " class='col-sm-12 badge-warning'>")
            .append($("<div>")
            .append($("<div class='p-1'>")
            .append($("<span id=" + $("#image-input-elem")[0].files[i].name.replace('.', '-') + "-status" + " class='spinner-border spinner-border-sm' role='status' aria-hidden='true'>"))
            .append($("<span class='pl-3'>").text($("#image-input-elem")[0].files[i].name)))));        
    }

    // Fájlok feltöltése és listázása a feltöltöttek sorában.
    //
    var data = $("#image-form")[0];
    var formData = new FormData(data);

    $.getJSON('/Employee/GetCurrentUserId', null, function(response) {

        var result = response;

        if (result.ItemId != null) 
        {
            var item = getUrlVars("estateId");

            var imageData = {
        
                UserId : result.ItemId,
                EstateId : item.estateId,
                Category : "",
                Title : "",
                DescriptionDetail : ""
            };

            formData.append('imageData', JSON.stringify(imageData));

            $.ajax(
                {
                    url : '/Estate/ImageUpload', 
                    type : 'POST',
                    data : formData,
                    processData: false,
                    contentType: false,

                    success : function(response) {

                        var result = JSON.parse(response);
                        
                        if (result.ItemId != null) 
                        {
                            for (var i = 0; i < $("#image-input-elem")[0].files.length; ++i)  {
                                
                                var uploaded = "";
                                var images = JSON.parse(result.Message);
                                for (var j = 0; j < images.length; ++j) {
                                    if (images[j].replace('.', '-') == $("#image-input-elem")[0].files[i].name.replace('.', '-'))
                                    {
                                        if (images[j].replace('.', '-') == $("#image-input-elem")[0].files[i].name.replace('.', '-')) 
                                        {
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).removeClass("badge-warning");
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).addClass("badge-success");
                                            $("#" + images[j].replace('.', '-') + "-status").removeClass("spinner-border spinner-border-sm");
                                            $("#" + images[j].replace('.', '-') + "-status").addClass('fa fa-check-circle');
                                        }
                                        else {
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).removeClass("badge-warning");
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).addClass("badge-danger");
                                            $("#" + images[j].replace('.', '-') + "-status").removeClass("spinner-border spinner-border-sm");
                                            $("#" + images[j].replace('.', '-') + "-status").addClass('fa fa-exclamation-triangle');
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }
    });
});

// Képfeltöltés fájlböngészés által.
//
$(document).on('click', '#image-upload-dropzone', function() {

    $('#image-input-elem').click();

});

$(document).on('change', '#image-input-elem', function() {
    
    $("#awaiting-for-upload-images-container")
            .append($("<div id=" + $("#image-input-elem")[0].files[0].name.replace('.', '-')  + " class='col-sm-12 badge-warning'>")
            .append($("<div>")
            .append($("<div class='p-1'>")
            .append($("<span id=" + $("#image-input-elem")[0].files[0].name.replace('.', '-') + "-status" + " class='spinner-border spinner-border-sm' role='status' aria-hidden='true'>"))
            .append($("<span class='pl-3'>").text($("#image-input-elem")[0].files[0].name)))));

    var data = $("#image-form")[0];
    var formData = new FormData(data);

    $.getJSON('/Employee/GetCurrentUserId', null, function(response) {

        var result = response;

        if (result.ItemId != null) 
        {
            var item = getUrlVars("estateId");

            var imageData = {
        
                UserId : result.ItemId,
                EstateId : item.estateId,
                Category : "",
                Title : "",
                DescriptionDetail : ""
            };

            formData.append('imageData', JSON.stringify(imageData));

            $.ajax(
                {
                    url : '/Estate/ImageUpload', 
                    type : 'POST',
                    data : formData,
                    processData: false,
                    contentType: false,

                    success : function(response) {

                        var result = JSON.parse(response);
                        if (result.ItemId != null)
                        {
                            for (var i = 0; i < $("#image-input-elem")[0].files.length; ++i)  {
                                
                                var uploaded = "";
                                var images = JSON.parse(result.Message);
                                for (var j = 0; j < images.length; ++j) {
                                    if (images[j].replace('.', '-') == $("#image-input-elem")[0].files[i].name.replace('.', '-'))
                                    {
                                        if (images[j].replace('.', '-') == $("#image-input-elem")[0].files[i].name.replace('.', '-')) 
                                        {
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).removeClass("badge-warning");
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).addClass("badge-success");
                                            $("#" + images[j].replace('.', '-') + "-status").removeClass("spinner-border spinner-border-sm");
                                            $("#" + images[j].replace('.', '-') + "-status").addClass('fa fa-check-circle');
                                        }
                                        else {
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).removeClass("badge-warning");
                                            $("#" + $("#image-input-elem")[0].files[i].name.replace('.', '-')).addClass("badge-danger");
                                            $("#" + images[j].replace('.', '-') + "-status").removeClass("spinner-border spinner-border-sm");
                                            $("#" + images[j].replace('.', '-') + "-status").addClass('fa fa-exclamation-triangle');
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            );
        }
    });

});

// Képfeltöltés befejezése.
//
$(document).on('click', '#finish-img-upload-btn', function() {
    window.location.reload();
});

// Ingatlanok gomb.
//
$(document).on('click', '#estates-btn', function() {

    $("#contents").empty();
    loadEstates();

});

// Ingatlan adatlap megjelenítés.
//
$(document).on('click', '#show-estate-detailsheet-btn', function() {

    if ($('#estate-detailsheet').is(':visible')) {
        $('#estate-detailsheet').hide(1000);
        $('#show-estate-detailsheet-btn').text('Adatlap Megtekintése');
    }
    else {
        var id = getUrlVars()["estateId"];
        $.getJSON('/Estate/Detail/?estateId=' + id, null, function(response) {

            $('#estate-detailsheet-price').text(response.Estate.Price + " HUF");
            $('#estate-detailsheet-area').text(response.Estate.TotalSquareFeet + " négyzetméter");
            $('#estate-detailsheet-category').text(response.Estate.Category);
            $('#estate-detailsheet-builtat').text(response.Estate.BuiltAt.split('-')[0]);
            $('#estate-detailsheet-refurbishedat').text(response.Estate.RefurbishedAt.split('-')[0]);

            $('#estate-detailsheet-quality').text(response.Estate.Quality);
            $('#estate-detailsheet-roomcount').text(response.Estate.RoomCount + " darab");
            $('#estate-detailsheet-kitchencount').text(response.Estate.KitchenCount + " darab");
            $('#estate-detailsheet-bathcount').text(response.Estate.BathroomCount + " darab");
            $('#estate-detailsheet-floorcount').text(response.Estate.FloorCount + " darab");

            $('#estate-detailsheet-landarea').text(response.Estate.LandSquareFeet + " négyzetméter");
            $('#estate-detailsheet-garagearea').text(response.Estate.GardenSquareFeet + " négyzetméter");
            $('#estate-detailsheet-gardenarea').text(response.Estate.GarageSquareFeet + " négyzetméter");
            $('#estate-detailsheet-teracearea').text(response.Estate.TerraceSquareFeet + " négyzetméter");
            $('#estate-detailsheet-roof').text(response.Estate.Roof);

            $('#estate-detailsheet-comfort').text(response.Estate.Comfort);
            $('#estate-detailsheet-outlook').text(response.Estate.Outlook);
            $('#estate-detailsheet-neighboor').text(response.Estate.Area);

        });

        $('#estate-detailsheet').show(1000);
        $('#show-estate-detailsheet-btn').text('Adatlap Elrejtése');
    }

});

// Hirdetés szerkesztése.
//
$(document).on('click', '#edit-estate-btn', function() {

    var estateId = getUrlVars()["estateId"];

    $.get('/Estate/Detail?estateId=' + estateId, null, function(response) {

        var result = JSON.parse(response);

        if (result != null)
        {
            $("#edit-estate-price").val(result.Estate.Price);
            $("#edit-estate-squarefeet").val(result.Estate.TotalSquareFeet);
            $("#edit-estate-category").val(result.Estate.Category);
            $("#edit-estate-builtat").val(result.Estate.BuiltAt);
            $("#edit-estate-refurbishedat").val(result.Estate.RefurbishedAt);
            $("#edit-estate-quality").val(result.Estate.Quality);
            $("#edit-estate-roomcount").val(result.Estate.RoomCount);
            $("#edit-estate-kitchencount").val(result.Estate.KitchenCount);
            $("#edit-estate-bathroomcount").val(result.Estate.BathroomCount);
            $("#edit-estate-floorcount").val(result.Estate.FloorCount);
            $("#edit-estate-propertysquarefeet").val(result.Estate.LandSquareFeet);
            $("#edit-estate-garagesquarefeet").val(result.Estate.GarageSquareFeet);
            $("#edit-estate-gardensquarefeet").val(result.Estate.GardenSquareFeet);
            $("#edit-estate-terracesquarefeet").val(result.Estate.TerraceSquareFeet);
            $("#edit-estate-roof").val(result.Estate.Roof);
            $("#edit-estate-comfort").val(result.Estate.Comfort);
            $("#edit-estate-view").val(result.Estate.Outlook);
            $("#edit-estate-area").val(result.Estate.Area);

            $("#edit-estate-hasgarage").attr('value', result.Estate.HasGarage);
            $("#edit-estate-haselevator").attr('value', result.Estate.HasElevator);
            $("#edit-estate-hasgarden").attr('value', result.Estate.HasGarden);

            $("#edit-estate-hasterrace").attr('value', result.Estate.HasTerace);
            $("#edit-estate-hasbasement").attr('value', result.Estate.HasBasement);
            $("#edit-estate-disabilityfriendly").attr('value', result.Estate.HasDisabledFriendly);

            $("#edit-estate-innerheightgreaterthan3meters").attr('value', result.Estate.HasInnerHeightGreatherThan3Meters);
            $("#edit-estate-hasseparatebathandtoilet").attr('value', result.Estate.HasSeparateWcAndBathroom);
            $("#edit-estate-haspanelprogramparticipation").attr('value', result.Estate.HasParticipatedInThePanelProgram);

            $("#edit-estate-hasheatisolation").attr('value', result.Estate.HasHeatIsolated);
            $("#edit-estate-hascirculation").attr('value', result.Heating.ByCirculation);
            $("#edit-estate-hasconvector").attr('value', result.Heating.ByGasConvector);

            $("#edit-estate-hasgasheating").attr('value', result.Heating.ByGas);
            $("#edit-estate-hasnetworkedheating").attr('value', result.Heating.ByNetworked);
            $("#edit-estate-haswoodenheating").attr('value', result.Heating.ByWood);

            $("#edit-estate-hascombinedheating").attr('value', result.Heating.ByCombined);
            $("#edit-estate-hasremoteheating").attr('value', result.Heating.ByRemote);
            $("#edit-estate-hasgasremote").attr('value', result.Heating.ByRemote);

            $("#edit-estate-haselectricityheating").attr('value', result.Heating.ByElectricity);
            $("#edit-estate-hascockleheating").attr('value', result.Heating.ByCockle);
            $("#edit-estate-haschimneyheating").attr('value', result.Heating.ByChimney);

            $("#edit-estate-hasfloorheating").attr('value', result.Heating.FloorHeating);
            //$("#edit-estate-hasairconditioner").attr('value', result.Heating.
            $("#edit-estate-hassuncollector").attr('value', result.Electricity.SunCollector);
            $("#edit-estate-networkedelectricity").attr('value', result.Electricity.Networked);

            $("#edit-estate-haspowerwall").attr('value', result.Electricity.PowerWall);
            $("#edit-estate-hasgrocerynearby").attr('value', result.Services.HasGroceryNearby);
            $("#edit-estate-haspetrolstationnearby").attr('value', result.Services.HasGasStationNearby);

            $("#edit-estate-haspublictransportnearby").attr('value', result.Services.HasTransportNearby);
            $("#edit-estate-hashaspharmacynearby").attr('value', result.Services.HasDrugStoreNearby);
            $("#edit-estate-hasschoolnearby").attr('value', result.Services.HasSchoolNearby);

            $("#edit-estate-hasentertainmentnearby").attr('value', result.Services.HasEntertainmentServicesNearby);
            $("#edit-estate-hasmaildepotnearby").attr('value', result.Services.HasMailDepotNearby);
            $("#edit-estate-hasbanknearby").attr('value', result.Services.HasBankNearby);

            $("#edit-estate-country").val(result.Address.Country);
            $("#edit-estate-county").val(result.Address.County);
            $("#edit-estate-city").val(result.Address.City);
            $("#edit-estate-postcode").val(result.Address.PostCode);
            $("#edit-estate-housenumber").val(result.Address.HouseNumber);
            $("#edit-estate-street").val(result.Address.Street);
            $("#edit-estate-floor").val(result.Address.FloorNumber);
            $("#edit-estate-door").val(result.Address.Door);
            $("#edit-estate-adtitle").val(result.Advertisement.Title);
            $("#edit-estate-addescr").val(result.Advertisement.DescriptionDetail);
            $("#edit-estate-adtype").val(result.Advertisement.OfferType);
            $("#edit-estate-priority").val(result.Advertisement.OrderOfAppearance);

            
            if (result.Estate.HasGarage)
                $("#edit-estate-hasgarage").addClass('tag-clicked');
            if (result.Estate.HasElevator)
                $("#edit-estate-haselevator").addClass('tag-clicked');
            if (result.Estate.HasGarden)
                $("#edit-estate-hasgarden").addClass('tag-clicked');                
            if (result.Estate.HasTerace)
                $("#edit-estate-hasterrace").addClass('tag-clicked');
            if (result.Estate.HasBasement)
                $("#edit-estate-hasbasement").addClass('tag-clicked');
            if (result.Estate.HasDisabledFriendly)
                $("#edit-estate-disabilityfriendly").addClass('tag-clicked');
            if (result.Estate.HasInnerHeightGreatherThan3Meters)
                $("#edit-estate-innerheightgreaterthan3meters").addClass('tag-clicked');
            if (result.Estate.HasSeparateWcAndBathroom)
                $("#edit-estate-hasseparatebathandtoilet").addClass('tag-clicked');
            if (result.Estate.HasParticipatedInThePanelProgram)
                $("#edit-estate-haspanelprogramparticipation").addClass('tag-clicked');                
            if (result.Heating.HasHeatIsolated)
                $("#edit-estate-hasheatisolation").addClass('tag-clicked');
            if (result.Heating.ByCirculation)
                $("#edit-estate-hascirculation").addClass('tag-clicked');
            if (result.Heating.ByGasConvector)
                $("#edit-estate-hasconvector").addClass('tag-clicked');
            if (result.Heating.ByGas)
                $("#edit-estate-hasgasheating").addClass('tag-clicked');
            if (result.Heating.ByNetworked)
                $("#edit-estate-hasnetworkedheating").addClass('tag-clicked');
            if (result.Heating.ByWood)
                $("#edit-estate-haswoodenheating").addClass('tag-clicked');                
            if (result.Heating.ByCombined)
                $("#edit-estate-hascombinedheating").addClass('tag-clicked');
            if (result.Heating.ByRemote)
                $("#edit-estate-hasremoteheating").addClass('tag-clicked');
            if (result.Heating.ByRemote)
                $("#edit-estate-hasgasremote").addClass('tag-clicked');
            if (result.Heating.ByElectricity)
                $("#edit-estate-haselectricityheating").addClass('tag-clicked');
            if (result.Heating.ByCockle)
                $("#edit-estate-hascockleheating").addClass('tag-clicked');
            if (result.Heating.ByChimney)
                $("#edit-estate-haschimneyheating").addClass('tag-clicked');                
            if (result.Heating.ByFloorHeating)
                $("#edit-estate-hasfloorheating").addClass('tag-clicked');
            if (result.Electricity.SunCollector)
                $("#edit-estate-hassuncollector").addClass('tag-clicked');
            if (result.Electricity.Networked)
                $("#edit-estate-networkedelectricity").addClass('tag-clicked');
            if (result.Electricity.PowerWall)
                $("#edit-estate-haspowerwall").addClass('tag-clicked');
            if (result.Services.HasGroceryNearby)
                $("#edit-estate-hasgrocerynearby").addClass('tag-clicked');                
            if (result.Services.HasGasStationNearby)
                $("#edit-estate-haspetrolstationnearby").addClass('tag-clicked');
            if (result.Services.HasTransportNearby)
                $("#edit-estate-haspublictransportnearby").addClass('tag-clicked');
            if (result.Services.HasDrugStoreNearby)
                $("#edit-estate-hashaspharmacynearby").addClass('tag-clicked');
            if (result.Services.HasSchoolNearby)
                $("#edit-estate-hasschoolnearby").addClass('tag-clicked');
            if (result.Services.HasEntertainmentServicesNearby)
                $("#edit-estate-hasentertainmentnearby").addClass('tag-clicked');
            if (result.Services.HasMailDepotNearby)
                $("#edit-estate-hasmaildepotnearby").addClass('tag-clicked');                
            if (result.Services.HasBankNearby)
                $("#edit-estate-hasbanknearby").addClass('tag-clicked');

        }

    });

});

// Hirdetés szerkesztésének elvetése.
//
$(document).on('click', '#cancel-edit-estate-btn', function() {

    $("#close-edit-dialog-btn").click();

});

// Szerkesztett hirdetési adatok jóváhagyása.
//
$(document).on('click', '#edit-estate-confirm-btn', function() {

    var updateTime = new Date();

    $.get('/Employee/GetCurrentUserId', null, function(response) {

        var result = JSON.parse(response);

        if (result.Success == true) {

            var advertiserId = result.ItemId;
            var estateId = getUrlVars()["estateId"];

            var estateObject = {

                Estate : {

                    Id : +estateId,
                    Price : +$("#edit-estate-price").val(),
                    TotalSquareFeet : +$("#edit-estate-squarefeet").val(),
                    LandSquareFeet : +$("#edit-estate-propertysquarefeet").val(),
                    RoomCount : +$("#edit-estate-roomcount").val(),
                    KitchenCount : +$("#edit-estate-kitchencount").val(),
                    BathroomCount : +$("#edit-estate-bathroomcount").val(),
                    FloorCount : +$("#edit-estate-floorcount").val(),
                    GarageSquareFeet : +$("#edit-estate-garagesquarefeet").val(),
                    GardenSquareFeet : +$("#edit-estate-gardensquarefeet").val(),
                    TerraceSquareFeet : +$("#edit-estate-terracesquarefeet").val(),
                    
                    BuiltAt : $("#edit-estate-builtat").val() + '-01-01',
                    RefurbishedAt : $("#edit-estate-refurbishedat").val() + '-01-01',    // TODO

                    HasElevator : $("#edit-estate-haselevator").attr('value'),
                    HasGarden : $("#edit-estate-hasgarden").attr('value'),
                    HasGarage : $("#edit-estate-hasgarage").attr('value'),
                    HasDisabledFriendly : $("#edit-estate-disabilityfriendly").attr('value'),
                    HasInnerHeightGreatherThan3Meters : $("#edit-estate-innerheightgreaterthan3meters").attr('value'),
                    HasSeparateWcAndBathroom : $("#edit-estate-hasseparatebathandtoilet").attr('value'),
                    HasParticipatedInThePanelProgram : $("#edit-estate-haspanelprogramparticipation").attr('value'),
                    HasHeatIsolated : $("#edit-estate-hasheatisolation").attr('value'),
                    HasTerace : $("#edit-estate-hasterrace").attr('value'),
                    HasBasement : $("#edit-estate-hasbasement").attr('value'),

                    Roof : $("#edit-estate-roof").val(),
                    Comfort : $("#edit-estate-comfort").val(),
                    Outlook : $("#edit-estate-view").val(),
                    Area : $("#edit-estate-area").val(),
                    Quality : $("#edit-estate-quality").val(),
                    Category : $("#edit-estate-category").val(),
                    AdvertiserId : advertiserId
                },
        
                Address : {

                    Id : 0,
                    FloorNumber : $("#edit-estate-floorcount").val(),
                    EstateId : +estateId,
                    
                    Country : $("#edit-estate-country").val(),
                    County : $("#edit-estate-county").val(),
                    City : $("#edit-estate-city").val(),
                    PostCode : $("#edit-estate-postcode").val(),
                    Street : $("#edit-estate-street").val(),
                    HouseNumber : $("#edit-estate-housenumber").val(),
                    Door : $("#edit-estate-door").val()
                },
        
                Electricity : {

                    Id : 0,
                    EstateId : +estateId,
                    
                    SunCollector : $("#edit-estate-hassuncollector").attr('value'),
                    PowerWall : $("#edit-estate-haspowerwall").attr('value'),
                    Networked : $("#edit-estate-networkedelectricity").attr('value'),
                },
        
                Heating : {

                    Id : 0,       
                    EstateId : +estateId,

                    ByCirculation : $("#edit-estate-hascirculation").attr('value'),
                    ByGasConvector : $("#edit-estate-hasconvector").attr('value'),
                    ByGas : $("#edit-estate-hasgasheating").attr('value'),
                    ByWood : $("#edit-estate-haswoodenheating").attr('value'),
                    ByCombined : $("#edit-estate-hascombinedheating").attr('value'),
                    ByChimney : $("#edit-estate-haschimneyheating").attr('value'),
                    ByCockle : $("#edit-estate-hascockleheating").attr('value'),
                    ByRemote : $("#edit-estate-hasremoteheating").attr('value'),
                    ByNetworked : $("#edit-estate-hasgasremote").attr('value'),
                    ByElectricity : $("#edit-estate-haselectricityheating").attr('value'),
                    ByFloorHeating : $("#edit-estate-hasfloorheating").attr('value')
                },
        
                PublicService : {

                    Id : 0,
                    EstateId : +estateId,
                    
                    HasGroceryNearby : $("#edit-estate-hasgrocerynearby").attr('value'),
                    HasGasStationNearby : $("#edit-estate-haspetrolstationnearby").attr('value'),
                    HasTransportNearby : $("#edit-estate-haspublictransportnearby").attr('value'),
                    HasDrugStoreNearby : $("#edit-estate-hashaspharmacynearby").attr('value'),
                    HasSchoolNearby : $("#edit-estate-hasschoolnearby").attr('value'),
                    HasMailDepotNearby : $("#edit-estate-hasmaildepotnearby").attr('value'),
                    HasBankNearby : $("#edit-estate-hasbanknearby").attr('value'),
                    HasEntertainmentServicesNearby : $("#edit-estate-hasentertainmentnearby").attr('value')
                },
        
                Advertisement : {

                    Id : 0,
                    OrderOfAppearance : +$('#edit-estate-priority').val(),
                    EstateId : +estateId,
                    
                    TimePosted : creationTime.getFullYear() + '-' + creationTime.getMonth() + '-' + creationTime.getDay(),
                    LastModification : null,

                    Title : $("#edit-estate-adtitle").val(),
                    DescriptionDetail : $("#edit-estate-addescr").val(),
                    OfferType : $("#edit-estate-adtype").val(),
                    AdvertiserId : advertiserId
                } 
            };            

            var requestBody = JSON.stringify(estateObject);

            $.post('/Estate/Modify', requestBody, function(response) {

                var result = JSON.parse(response);

                if (result.Success == true) {
                    alert('sikeres módosítás');
                }
                else
                    alert('módosítás sikertelen');
            });
        }
    });

});

// Képgaléria nyitása.
//
var gallery;
var galleryIndex;
var clickedPrev;
var clickedNext;

function openGallery(i) {

    $('#img-gallery-content').attr('src', gallery[i]);
    $('#image-gallery-modal').modal('toggle')
    ++galleryIndex;
    
}

// Képgaléria tartalom betöltés.
//
$(document).on('click', '#image-gallery', function(item) {

    var id = getUrlVars()["estateId"];
    $.get('/Estate/Detail?estateId=' + id, null, function(response) {

        var result = JSON.parse(response);

        if (result != null) {

            var images = result.Images;

            // Visszaállítás.
            //
            gallery = new Array();
            galleryIndex = 0;
            clickedPrev = false;
            clickedNext = false;

            for (var i = 0; i < images.length; ++i) 
            {
                var path = '/static/' + images[i].Id + images[i].Extension;
                gallery.push(path);
            }

            openGallery(galleryIndex);
        }

    });

});

// Következő kép.
//
$(document).on('click', '#next-gallery-item', nextImage);
function nextImage() {

    clickedNext = true;
    if (clickedPrev == true) {
        galleryIndex+=2;
        clickedPrev = false;
    }

    if (galleryIndex < gallery.length) {
        $('#img-gallery-content').attr('src', gallery[galleryIndex]);
        ++galleryIndex;
    }
    else 
        galleryIndex = gallery.length - 1;

}

// Előző kép.
//
$(document).on('click', '#prev-gallery-item', prevImage);
function prevImage() {

    clickedPrev = true;
    if (clickedNext == true) {
        galleryIndex -= 2;
        clickedNext = false;
    }

    if (galleryIndex >= 0) {
        $('#img-gallery-content').attr('src', gallery[galleryIndex]);
        --galleryIndex;
    }
    else 
        galleryIndex = 0;

}

// Billentyűzet nyilak.
//
$(document).keydown(function(key) {
   
    switch(key.which) {
        case 37: prevImage();
        break;
        case 39: nextImage();
        break;
    }

});

// Rendezés.
//
var highestPrice = function(estates) {

    for(var i = 0; i < estates.length; ++i) 
    {
        var max = estates[i];
    
        for (var j = i; j < estates.length; ++j) 
        {
            if (estates[j].Price > max.Price) {

                var temp = estates[j];
                estates[j] = estates[i];
                estates[i] = temp;
            }
        }
    }

    return estates;
};

var lowestPrice = function(estates) {

    for(var i = 0; i < estates.length; ++i) 
    {
        var min = estates[i];
    
        for (var j = i; j < estates.length; ++j) 
        {
            if (estates[j].Price < min.Price) {

                var temp = estates[j];
                estates[j] = estates[i];
                estates[i] = temp;
            }
        }
    }

    return estates;
};

var newestDate = function(estates) {

    for(var i = 0; i < estates.length; ++i) 
    {
        first = new Date(estates[i].TimePosted);
    
        for (var j = i; j < estates.length; ++j) 
        {
            var second = new Date(estates[j].TimePosted);
            if (second > first) {

                var temp = estates[j];
                estates[j] = estates[i];
                estates[i] = temp;
            }
        }
    }

    return estates;
};

var oldestDate = function(estates) {

    for(var i = 0; i < estates.length; ++i) 
    {
        first = new Date(estates[i].TimePosted);
    
        for (var j = i; j < estates.length; ++j) 
        {
            var second = new Date(estates[j].TimePosted);
            if (second < first) {

                var temp = estates[j];
                estates[j] = estates[i];
                estates[i] = temp;
            }
        }
    }

    return estates;
};

// Legdrágább elöl.
//
$(document).on('click', '#cheapest-first-btn', function() {

    window.location.href = '/estates.html?orderBy=expensiveFirst';

});

function expensive() {

    $('#cheapest-first-btn').hide();
    $('#expensive-first-btn').show();

    $('#expensive-first-btn').addClass('alert-success');

    $('#cheapest-first-btn').removeClass('alert-success');
    $('#newest-first-btn').removeClass('alert-success');
    $('#oldest-first-btn').removeClass('alert-success');
}

// Legolcsóbb elöl.
//
$(document).on('click', '#expensive-first-btn', function() {

    window.location.href = '/estates.html?orderBy=cheapestFirst';

});

function cheapest() {
    $('#expensive-first-btn').hide();
    $('#cheapest-first-btn').show();

    $('#cheapest-first-btn').addClass('alert-success');

    $('#expensive-first-btn').removeClass('alert-success');
    $('#newest-first-btn').removeClass('alert-success');
    $('#oldest-first-btn').removeClass('alert-success');
}

// Legrégebbi elöl.
//
$(document).on('click', '#newest-first-btn', function() {

    window.location.href = '/estates.html?orderBy=oldestFirst';

});

function oldest() {

    $('#newest-first-btn').hide();
    $('#oldest-first-btn').show();

    $('#oldest-first-btn').addClass('alert-success');

    $('#expensive-first-btn').removeClass('alert-success');
    $('#newest-first-btn').removeClass('alert-success');
    $('#cheapest-first-btn').removeClass('alert-success');

}

// Legújabb elöl.
//
$(document).on('click', '#oldest-first-btn', function() {

    window.location.href = '/estates.html?orderBy=newestFirst';
    
});

function newest() {

    $('#oldest-first-btn').hide();
    $('#newest-first-btn').show();

    $('#newest-first-btn').addClass('alert-success');

    $('#expensive-first-btn').removeClass('alert-success');
    $('#oldest-first-btn').removeClass('alert-success');
    $('#cheapest-first-btn').removeClass('alert-success');

}

// Ügyfél el akar adni.
//
$(document).on('click', '#send-seller-request-btn', function() {

    var clientRequest = {

        Id: 0,
        Name: $('#cust-name').val(),
        Email: $('#cust-email').val(),
        Phone: $('#cust-phone').val(),
        Message: $('#cust-message').val(),
        ReasonSellRequest: +$('#reason-sell-request').attr('value'),
        ReasonPhotoRequest: +$('#reason-photo-request').attr('value'),
        ReasonPriceCheckRequest: +$('#reason-pricecheck-request').attr('value'),
        ReasonAdviceRequest: +$('#reason-advice-request').attr('value')

    };

    if (clientRequest.Name != null && clientRequest.Email != null && clientRequest.Phone != null && clientRequest.Message != null) {

        var json = JSON.stringify(clientRequest);

        $.post('/Seller/EstateSellRequest', json, function(response) {

            var result = JSON.parse(response);

            if (result.Success == true) {

                alert(result.Message);
                $('#close-sell-estate-modal').click();
            }
            else {
                alert(result.Message);
            }
        });
    }

});

// További ingatlanok gomb.
//
$(document).on('click', '#more-estate-btn', function() {

    location.href = '/estates.html';

});

// Munkatárs regisztráció megszakítás.
//
$(document).on('click', '#cancel-user-registration-btn', function() {

    $('#close-user-registration-dialog-btn').click();

});

// Munkatárs regisztráció.
//
$(document).on('click', '#register-user-btn', function() {

    $('#new-user-role').empty();
    $.getJSON('/Employee/GetRoles', null, function(response) {

        for (var i = 0; i < response.length; ++i)
            $('#new-user-role').append("<option>" + response[i] + "</option>");
    });

});

// Munkatárs regisztráció megerősítés.
//
$(document).on('click', '#confirm-register-user-btn', function() {

    var registration = {

        Email: $('#new-user-email').val(),
        Phone: $('#new-user-phone').val(),
        FirstName: $('#new-user-firstname').val(),
        MiddleName: $('#new-user-middlename').val(),
        LastName: $('#new-user-lastname').val(),
        RoleTitle: $('#new-user-role').val(),
        Description: $('#new-user-description').val()

    };

    $.post('/Employee/Registration', JSON.stringify(registration), function(response) {

        var result = JSON.parse(response);

        if (result.Success == true) {
            alert('sikeres regisztráció..');
        }

    });

});


// Kereső megjelenítése.
//
$(document).on('click', '#search-btn', function() {

    $('.search-btn-container').hide(500);
    $('#search-module-container').show(1000);

});

// Kereső elrejtése.
//
$(document).on('click', '#hide-simple-search-container', function() {

    $('#search-module-container').hide(500);
    $('.search-btn-container').show(1000);

});

// Adatbázis adatok törlése - megszakítás.
//
$(document).on('click', '#cancel-erease-database-btn', function() {

    $('#close-erease-database-btn').click();

});

// Adatbázis adatok törlése - jóváhagyás.
//
$(document).on('click', '#confirm-erease-database-btn', function() {

    $('#dbase-erease-alert').hide(500);
    $('#dbase-erease-inprogress').show(1000);

    $.getJSON('/Application/FactoryReset', null, function(response) {

        if (response.Success == true) {
            $('#dbase-erease-inprogress').hide(500);
            $('#dbase-erease-success').show(1000);
            window.setTimeout(function() {
                window.location.href = '/home.html';
            }, 2000);
        }
        else {
            $('#dbase-erease-inprogress').hide(500);
            $('#dbase-erease-error').show(1000);
            window.setTimeout(function() {
                window.location.href = '/home.html';
            }, 2000);
        }

    });

});

// Új jogosultság hozzáadása - megszakítás.
//
$(document).on('click', '#cancel-new-role-btn', function() {

    $('#close-new-role-btn').click();

});

// Új jogosultság hozzáadása.
//
$(document).on('click', '#confirm-new-role-btn', function() {

    var role = $('#role-name').val();
    $.post('/Employee/NewRole?roleTitle=' + role, null, function(response) {

        var result = JSON.parse(response);
        if (result.Success == true) {
            $('#role-added-message').show(1000);
            window.setTimeout(function() {
                window.location.href = '/admin.html';
            }, 2000);
        }

    });

});