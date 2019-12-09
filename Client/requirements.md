# Summary

    Author:         David Teket
    Created:        2018-11-10
    Description:    This document helps me collecting and revising presentation requirements while proceeding
                    with the development of the application's client-side.
    
    github:         github.com/davidteket

# Front-end

Az alkalmazás a szerveroldali webapi-ra épít és annak rendelkezésre álló szolgáltatásait használja.
A kommunikáció JSON alapú,  a sablonok és egyes adatok betöltése dinamikus lehet.

A kliens reszponzív (mobiloptimalizált) ami azt jelenti, hogy az egyes felületelemek
minimálisan vagy meg sem jelennek egyes felbontások alatt.

# index.html

- az alkalmazás kezdőoldala, a következőket kell tartalmaznia:

    - bejelentkezés és elérhetőség-sáv (osztott)
    - felső menüsáv ami az alkalmazáson belüli navigálást teszi lehetővé (osztott)
    - kép-slider (sablon)
    - keresőmodul (osztott)
    - ingatlan-listázó (osztott)
    - munkatárs-listázó (osztott)
    - alsó menüsáv ami az alkalmazással és a céggel kapcsolatos információkat nyújtja (osztott)

# site/estate_details.html

- adott ingatlant részletező oldal:

    - bejelentkezés és elérhetőség-sáv (osztott)
    - felső menüsáv ami az alkalmazáson belüli navigálást teszi lehetővé (osztott)

    - részletező modul: 

        - képnézegető (sablon)
        - specifikáció
        - leírás
        - hirdető box
        - áttekintés box
        - kérdés box

    - alatta hirdetés-infó sáv
    - alatta openstreetmap ami kijelöli az ingatlant a térképen
    - alsó menüsáv ami az alkalmazással és a céggel kapcsolatos infókat nyújtja (osztott)


# site/employee_details.html

- adott munkatársat részletező oldal:

    - bejelentkezés és elérhetőség-sáv (osztott)
    - az adott munkatársat részletező oldal:

        - felső menüsáv ami az alkalmazáson belüli navigálást teszi lehetővé (osztott)
        - profilkép box
        - elérhetőség és általános infó box
        - bemutatkozás box
        - kínálatok megtekintése-box

    - alsó menüsáv ami az alkalmazással és a céggel kapcsolatos infókat nyújtja (osztott)

# site/registration.html

- munkatárs regisztrációs oldal:

    - bejelentkezés és elérhetőség-sáv (osztott)
    - felső menüsáv ami az alkalmazáson belüli navigálást teszi lehetővé (osztott)
    - regisztrációs form
    - képfeltöltés-felület (sablon)
    - alsó menüsáv ami az alkalmazással és a céggel kapcsolatos infókat nyújtja (osztott)

# site/login.html

- felső menüsáv ami az alkalmazáson belüli navigálást teszi lehetővé (osztott)

- munkatárs bejelentkeztető oldal:

    - háttérkép
    - felhasználói-név
    - jelszó
    - elfelejtettem a jelszavamat
    - regisztrációs lépések

- alsó menüsáv ami az alkalmazással és a céggel kapcsolatos infókat nyújtja (osztott)

# site/upload.html

- ingatlanhirdetés feladó oldal:

    - bejelentkezés és elérhetőség-sáv (osztott)
    - felső menüsáv ami az alkalmazáson belüli navigálást teszi lehetővé (osztott)
    - ingatlanhirdetés-form
    - képfeltöltés-felület (sablon)
    - alsó menüsáv ami az alkalmazással és a céggel kapcsolatos infókat nyújtja (osztott)

# site/shared

## login_strip.html

- bejelentkezés és elérhetőség sáv

    - telefonszám
    - cím
    - email
    - dolgozói belépés

## menu_strip.html

- felső menüsáv az alkalmazáson belüli navigációhoz

    - cégnév/logó
    - kezdőlap
    - ingatlanok
    - keresési megbízás
    - munkatársak
    - kapcsolat
    - rólunk

# image_slider.html

- kiemelt hirdetések automatikus megtekintése
- nyilakkal léptethető

- a következőket jeleníti meg:

    - a hirdetés elsődleges képe
    - hirdetés címe
    - ár

- kattintásra behozza az ingatlan részletezőt

## search_module.html

- keresőmodul mind egyszerű és összetett kereséshez
- egyszerű:

    - ingatlan típusa
    - ár (tól-ig)
    - hely

- összetett:

    - minden adatpontra lehet kritériumot megfogalmazni ami az ingatlan-hirdetés formon megjelenik
      és az adatbázis által támogatott.

- javaslati alapon működik (legördülők és javaslatok)
- dinamikusan tölti be a találatokat egy megadott konténerbe

## estate_lister.html

- ingatlanlistázó több különböző nézet támogatásával

    - lista (kis részletességű tételek)
    - mozaik (közepes részletességű tételek)
    - rács (részletes tételek)

## employee_lister.html

- munkatárs listázó

    - rács nézetben jeleníti meg a dolgozókat

## footer_strip.html

- céginfó és elérhetőség

# konténerek (css)

- minden modul, sablon és tartalom megjelenítő külön konténert alkosson:

## header-login-container.css
## header-menu-container.css
## page-content-container.css
## footer-menu-container.css
## image-slider-container.css
## search-module-container.css
## estate-lister-container.css
## employee-lister-container.css
## estate-listview-container.css
## estate-thumbnailview-container.css
## estate-gridview-container.css
## employee-gridview-container.css


    