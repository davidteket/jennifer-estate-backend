# Summary

    Author:         David Teket
    Created:        2018-10-23
    Description:    This document helps me collecting and revising WebAPI requirements while proceeding
                    with the development of the application's server-side.
    
    github:         github.com/davidteket

# API endpoints

## GET (the ones that gives back JSON data along with the view)
    
    /ingatlan

        /ingatlan/id/reszletek

    /rolunk
    /munkatarsak

        /munkatarsak/id/profil

    /ingatlankeresesi-megbizas
    /rolunk
    /belepes

## POST (the ones that post JSON data to the server from the view)

    /munkatarsak/id/profil
    /ingatlan/feltoltes
    /munkatarsak/regisztracio
    /belepes
    /ingatlankeresesi-megbizas

# JSON responses - HTTP GET

`/ingatlan`

- minden esetben lekérnek egy meghatározott számú ingatlant. a válasz formátuma a következő:

    {
        "EstateId" : 5678,
        "Image" : 1234,
        "Description" : "Lorem Ipsum",
        "Price" : 8765432,
        "OfferType" : "Eladó",
        "City" : "Budapest",
        "PostCode" : 1055,
        "Street" : "Kerepesi Út",
        "Country" : "Magyarország"
    }

- az /index -en tartózkodva bármely ingatlanra kattintva átirányítódunk a
  reszletek oldalra:

`/ingatlan/id/reszletek`

- ekkor a válasz tartalmazza az ingatlanhoz tartozó teljes részletességű leírást:

    {
        "SquareFeet" : 2544,
        "Category" : "Lakás",
        "BuiltAt" : 2005-08-10,
        "RefurbishedAt" : "",
        "Grade" : "Újszerű",
        "Room" : 4,
        "Kitchen" : 1,
        "Bathroom" : 2,
        "FloorCount" : 2,
        "Garage" : true,
        "Elevator" : false,
        "Terrace" : true,
        "PropertySquareFeet" : 3544,
        "GarageSquareFeet" : 60,
        "GardenSquareFeet" : 152,
        "TerraceSquareFeet" : 84,
        "Basement" : true,
        "Comfort" : "Összkomfortos",
        "Advertiser" : "Nagy Katalin",
        "AdvertiserId : 6
    }

- még ugyanezen az oldalon, viszont opcionális, ajaxos kéréssel visszakapható adatok az ingatlan
  egyes technikai oldalait tekintve:

Fűtés:

    {
        "ByWood" : true,
        "ByRemote" : false,
        "ByGas" : true,
        "ByElectricity" : false,
        "FloorHeating" : false
    }
    
Áramellátás:

    {
        "SunCollector" : false,
        "Thermal" : false,
        "Networked" : true,

    }

Szolgáltatás:

    {
        "Grocery" : 40,
        "GasStation" : 240,
        "Transport" : 20,
        "DrugStore" : 382,
        "School" : 690,
        "MailDepot" : 695,
        "Bank" : 547
    }

`/munkatarsak`

- listázza a cég dolgozóit:

    {
        "EmployeeId" : 6,
        "FirstName" : "Katalin",
        "LastName" : "Nagy",
        "Phone" : 36701234567,
        "Email" : "katalin.nagy@websitename.com",
        "ProfilePicture" : 145
    }

- ezen felül bármely dolgozóra kattintva, az adott ingatlanos profilja töltődik be,
  további részletekkel:

`/munkatarsak/id/profil`

    {
        "Description" : "lorem ipsum lorem ipsum",
        "Advertisements" : [12, 20, 8, 4, 7, 9]
    }

- itt a "megnézem a hirdetéseit" gombra kattintva listázódnak az ingatlanok a /ingatlan elérésnél ismertetett módon
  amíg ez nem történik meg, addig csak egy számot fog kiírni, hogy mennyi hirdetése van az ingatlanosnak (6 darab)

# JSON responses - HTTP POST

`/munkatarsak/id/profil`

- ha az adott munkatárs be van lépve, akkor szerkesztheti a profilját, ahol az alábbiakat módosíthatja:

    {
        "FirstName" : "Katalin",
        "MiddleName" : "",
        "LastName" : "Nagy",
        "ApproachType" "Dr.",
        "Phone" : 36701234567,
        "Email" : "katalin.nagy@websitename.com",
        "ProfilePicture" : 145
    }

`/regisztracio`

- itt munkatárs regisztrálhat munkatársat - vagy email-es meghívásos alapon idelépett látogató is regisztrálhatja magát
  az oldal munkatársaként. a következő adatokat adhatja meg:

    {
        "FirstName" : "Katalin",
        "MiddleName" : "",
        "LastName" : "Nagy",
        "ApproachType" : "",
        "Phone" : 36701234567,
        "Email" : "katalin.nagy@websitename.com",
    }

`/invitacio`

- meglévő munkatárs hívhat meg regisztrációra egy új munkatársat. 
  a meghívás gomb a belépett munkatárs profilján kersztül érhető el

    {
        "Email" : "asdf@websitename.com"
    }

`/belepes`

- itt munkatárs léphet be a hozzá tartozó email és jelszó megadásával:

    {
        "Email" : "katalin.nagy@websitename.com",
        "Password" : "pw1234"
    }


`/feltoltes`

- ha az adott munkatárs be van lépve, akkor a főoldalon és és az ingatlanok, vagy ingatlan-részletek oldalon
  látni fog egy ikont melyre kattintva átirányítódik a fenti url-re.
  itt egy új ingatlant tölthet fel, melyre a következő adatpontokat adhatja meg:

`estate` : 

        public int Id { get; set; }
        public int Price { get; set; }
        public int SquareFeet { get; set; }
        public string Category { get; set; }
        public DateTime BuiltAt { get; set; }
        public DateTime? RefurbishedAt { get; set; }
        public string Grade { get; set; }
        public int Room { get; set; }
        public int Kitchen { get; set; }
        public int Bathroom { get; set; }
        public int FloorCount { get; set; }
        public int Garage { get; set; }
        public int Elevator { get; set; }
        public int Garden { get; set; }
        public int Terace { get; set; }
        public int PropertySquareFeet { get; set; }
        public int? GarageSquareFeet { get; set; }
        public int? GardenSquareFeet { get; set; }
        public int? TerraceSquareFeet { get; set; }
        public int Basement { get; set; }
        public string Comfort { get; set; }
        public int AdvertiserId { get; set; }

`JSON` :

{
    "Id" : 0,
    "Price" : 8765432,
    "SquareFeet" : 2544,
    "Category" : "Lakás",
    "BuiltAt" : 2005-08-10,
    "RefurbishedAt" : "",
    "Grade" : "Újszerű",
    "Room" : 4,
    "Kitchen" : 1,
    "Bathroom" : 2,
    "FloorCount" : 2,
    "Garage" : true,
    "Elevator" : false,
    "Garden" : true,
    "Terace" : true,
    "PropertySquareFeet" : 3544,
    "GarageSquareFeet" : 60,
    "GardenSquareFeet" : 152,
    "TerraceSquareFeet" : 84,
    "Basement" : true,
    "Comfort" : "Összkomfortos",
    "AdvertiserId : 6
}



`address` : 

        public int Id { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Door { get; set; }
        public int? FloorNumber { get; set; }
        public int EstateId { get; set; }

`JSON` : 

{
    "Id" : 0,
    "Country" : "Magyarország",
    "County" : "Pest",
    "City" : "Budapest",
    "PostCode" : 1055,
    "Street" : "Kerepesi Út",
    "HouseNumber" : 13,
    "Door" : 5,
    "FloorNumber" : null,
    "EstateId" : 0
}


`electricity` :

        public int Id { get; set; }
        public bool SunCollector { get; set; }
        public bool Thermal { get; set; }
        public bool Networked { get; set; }
        public int EstateId { get; set; }


`JSON` : 

{
    "Id" : 0,
    "SunCollector" : true,
    "Thermal" : false,
    "Networked" : true,
    "EstateId" : 0
}


`heating` : 

        public int Id { get; set; }
        public bool ByWood { get; set; }
        public bool ByRemote { get; set; }
        public bool ByGas { get; set; }
        public bool ByElectricity { get; set; }
        public bool FloorHeating { get; set; }
        public int EstateId { get; set; }

`JSON` : 

{
    "Id" : 0,
    "ByWood" : true,
    "ByRemote" : false,
    "ByGas" : true,
    "ByElectricity" : false,
    "FloorHeating" : false,
    "EstateId" : 0 
}


`publicService` : 

        public int Id { get; set; }
        public int Grocery { get; set; }
        public int GasStation { get; set; }
        public string Transport { get; set; }
        public int DrugStore { get; set; }
        public int School { get; set; }
        public int MailDepot { get; set; }
        public int Bank { get; set; }
        public int EstateId { get; set; }

`JSON` : 

{
    "Id" : 0,
    "Grocery" : 54,
    "GasStation" : 402,
    "Transport" : "bkk,
    "DrugStore" : 94,
    "School" : 840,
    "MailDepot" : 1002,
    "Bank" : 521,
    "EstateId" : 0 
}


`water` : 

        public int Id { get; set; }
        public string AvailabilityType { get; set; }
        public int EstateId { get;  set; }

`JSON` : 

{
    "Id" : 0,
    "AvailabilityType" : "csatornazott",
    "EstateId" : 0
}


`advertisement` : 

        public int Id { get; set; }
        public DateTime TimePosted { get; set; }
        public string Title { get; set; }
        public string DescriptionDetail { get; set; }
        public DateTime? LastModification { get; set; }
        public int? OrderOfAppearance { get; set; }
        public string OfferType { get; set; }
        public int EstateId { get; set; }
        public int AdvertiserId { get; set; }


`JSON` : 

{
    "Id" : 0,
    "TimePosted" : 2019-11-06,
    "Title" : "teszt hirdetes",
    "DescriptionDetail" : "epitem a webappot akar a mintaprogramozo",
    "LastModification" : null,
    "OrderOfAppearance" : null,
    "OfferType" : "elado",
    "EstateId" 0: ,
    "AdvertiserId" : 6 
}

## To submit by the upload form

{
    "Estate" : {
        "Id" : 0,
        "Price" : 8765432,
        "SquareFeet" : 2544,
        "Category" : "Lakás",
        "BuiltAt" : "2005-08-10",
        "RefurbishedAt" : "",
        "Grade" : "Újszerű",
        "Room" : 4,
        "Kitchen" : 1,
        "Bathroom" : 2,
        "FloorCount" : 2,
        "Garage" : 1,
        "Elevator" : 0,
        "Garden" : 1,
        "Terace" : 1,
        "PropertySquareFeet" : 3544,
        "GarageSquareFeet" : 60,
        "GardenSquareFeet" : 152,
        "TerraceSquareFeet" : 84,
        "Basement" : 1,
        "Comfort" : "Összkomfortos",
        "AdvertiserId" : 6
    },
    "Address" : {
        "Id" : 0,
        "Country" : "Magyarország",
        "County" : "Pest",
        "City" : "Budapest",
        "PostCode" : 1055,
        "Street" : "Kerepesi Út",
        "HouseNumber" : 13,
        "Door" : 5,
        "FloorNumber" : null,
        "EstateId" : 0
    },
    "Electricity" : {
        "Id" : 0,
        "SunCollector" : 1,
        "Thermal" : 0,
        "Networked" : 1,
        "EstateId" : 0
    },
    "Heating" : {
        "Id" : 0,
        "ByWood" : 1,
        "ByRemote" : 0,
        "ByGas" : 1,
        "ByElectricity" : 0,
        "FloorHeating" : 0,
        "EstateId" : 0 
    },
    "PublicService" : {
        "Id" : 0,
        "Grocery" : 54,
        "GasStation" : 402,
        "Transport" : "bkk",
        "DrugStore" : 94,
        "School" : 840,
        "MailDepot" : 1002,
        "Bank" : 521,
        "EstateId" : 0 
    },
    "Water" : {
        "Id" : 0,
        "AvailabilityType" : "csatornazott",
        "EstateId" : 0
    },
    "Advertisement" : {
        "Id" : 0,
        "TimePosted" : "2019-11-06",
        "Title" : "teszt hirdetes",
        "DescriptionDetail" : "epitem a webappot akar a mintaprogramozo",
        "LastModification" : null,
        "OrderOfAppearance" : null,
        "OfferType" : "elado",
        "EstateId" : 0,
        "AdvertiserId" : 6 
    } 
}

`/ingatlan/id/modositas`

- ha egy munkatárs be van lépve és az adott id-jú ingatlan oldalon áll éppen,
  akkor megjelenik egy módosítás gomb, melyre kattintva az alábbi adatpontok módosítására nyíélik lehetőség:

## To submit by the modify form

 {
    "Estate" : {
        "Id" : 6,
        "Price" : 8765432,
        "SquareFeet" : 2544,
        "Category" : "Faház",
        "BuiltAt" : "2005-08-10",
        "RefurbishedAt" : "",
        "Grade" : "Régi",
        "Room" : 4,
        "Kitchen" : 1,
        "Bathroom" : 2,
        "FloorCount" : 2,
        "Garage" : 1,
        "Elevator" : 0,
        "Garden" : 1,
        "Terace" : 1,
        "PropertySquareFeet" : 3544,
        "GarageSquareFeet" : 60,
        "GardenSquareFeet" : 152,
        "TerraceSquareFeet" : 84,
        "Basement" : 1,
        "Comfort" : "kényelmes",
        "AdvertiserId" : 6
    },
    "Address" : {
        "Id" : 6,
        "Country" : "Németország",
        "County" : "Nurn",
        "City" : "Nürnberg",
        "PostCode" : 12345,
        "Street" : "Strasse str",
        "HouseNumber" : 55,
        "Door" : 5,
        "FloorNumber" : null,
        "EstateId" : 6
    },
    "Electricity" : {
        "Id" : 6,
        "SunCollector" : 1,
        "Thermal" : 0,
        "Networked" : 1,
        "EstateId" : 6
    },
    "Heating" : {
        "Id" : 6,
        "ByWood" : 0,
        "ByRemote" : 0,
        "ByGas" : 1,
        "ByElectricity" : 0,
        "FloorHeating" : 1,
        "EstateId" : 6 
    },
    "PublicService" : {
        "Id" : 6,
        "Grocery" : 54,
        "GasStation" : 402,
        "Transport" : "gertransport",
        "DrugStore" : 94,
        "School" : 840,
        "MailDepot" : 2002,
        "Bank" : 521,
        "EstateId" : 6 
    },
    "Water" : {
        "Id" : 6,
        "AvailabilityType" : "kutas",
        "EstateId" : 6
    },
    "Advertisement" : {
        "Id" : 6,
        "TimePosted" : "2019-11-06",
        "Title" : "módosított hirdetés",
        "DescriptionDetail" : "json egy nyelvet beszélünk",
        "LastModification" : null,
        "OrderOfAppearance" : null,
        "OfferType" : "eladó",
        "EstateId" : 6,
        "AdvertiserId" : 6 
    } 
}

`/ingatlankeresesi-megbizas`

- itt oldal látogató nyújthat be keresési megbízást a következő paraméterek alapján:

    {
        "FirstName" : "Katalin",
        "MiddleName" : "",
        "LastName" : "Nagy",
        "ApproachType" : "",
        "Email" : "katalin.nagy@websitename.com",
        "RequestCallback" : true",
        "Phone": 36701234567,
        "Category" : "Lakás",
        "Location" : "Budapest, V kerület",
        "Radius" : 5km,
        "MinPrice" : 10M,
        "MaxPrice" : 18M,
        "Note" : "fontos, hogy..."
        "AcceptedPrivacyPolicy" : true
    }

`/kapcsolatfelvetel`

- itt oldal látogató veheti fel a kapcsolatot a céggel: 

    {
        "FirstName" : "Katalin",
        "MiddleName" : "",
        "LastName" : "Nagy",
        "ApproachType" : "",
        "Email" : "katalin.nagy@websitename.com",
        "Subject" : "asdf",
        "Content" : "lorem ipsum lorem ipsum",
        "AcceptedPrivacyPolicy" : true
    }

`/ingatlan/id/torles`

- itt munkatárs törölheti az adott ingatlan hirdetését

    {
        "EstateId" : 123
    }

`/munkatarsak/id/torles`

- itt munkatars profil torlese lehetséges (csakis 1 acc képes erre)

    {
        "AdvertiserId" : 123
    }

# Adding user management

- az identity framework és a megfelelő adatbázistáblák hozzáadásával (lsd. `add_identity.sql`) a következő 
  URL-ek user login-hoz kötöttek.

  `/munkatarsak/regisztracio`

- új munkatárs regisztrációja mehet végbe:

        public string Id { get; set; }
        public string Email { get; set; }
        public int EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public int PhoneNumberConfirmed { get; set; }
        public int TwoFactorEnabled { get; set; }
        public System.DateTime LocoutEndUtc { get; set; }
        public int LockOutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }

`JSON`: 

{
    "User" : {
        "Email" : "asdf@gmail.com",
        "EmailConfirmed" : 1,
        "PasswordHash" : null,
        "SecurityStamp" : null,
        "PhoneNumber" : "123465",
        "PhoneNumberConfirmed" : 0,
        "TwoFactorEnabled" : 0,
        "LockOutEndUtc" : "2019-11-06",
        "LockOutEnabled" : 1,
        "AccessFailedCount" : 0,
        "UserName" : "davidteket11"
    },
    "Password" : "Valami777??"
}

# Finalizing the api

- minden végpont letesztelve
- a szolgáltatások működnek