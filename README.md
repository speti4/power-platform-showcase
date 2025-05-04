# Power Platform prezi guide

Az elmúlt hónapokban célzottan, rendszerszinten tanultam a Microsoft Power Platform technológiát, azzal a szándékkal, hogy ne csak App Maker, hanem **Developer és Solution Architect** szinten is átlássam a működését.

<details>
    <summary>
        <h2>Power Platform kompetenciák</h2>
    </summary>

![Microsoft Power Platform cert roadmap](docs/cert-roadmap.jpg)
[^1]

### PL-900: Microsoft Power Platform Fundamentals

Belépő szint. Ismeri a platform komponenseit, a core működést

### PL-100: Microsoft Power Platform App Maker (retired)

Képes egyszerű üzleti problémák megoldására alkalmazásokat, automatizmusokat léterhozni. Basic canvas és model-driven appok, alapvető Dataverse ismeretek

### PL-200: Microsoft Power Platform Functional Consultant

A funkcionális tanácsadó szint: canvas‑ és model‑driven appok építése, Power Automate‑folyamatok konfigurálása, Dataverse‑adatmodellezés és alap Power BI‑riportok – ide már gyakorlati megoldás‑tervezés és konfigurálás szükséges.

### PL-400: Microsoft Power Platform Developer

Fejlesztői szint: Alkalmazáséletciklus-kezelés (ALM) és DevOps gyakorlatok alkalmazása, ismeri és használja a Power Platform teljes eszköztárát, képes kiterjeszteni pro-codedal - .NET (C#) és JavaScript

- API-k és egyéni csatlakozók integrálása
- egyéni plug‑inek és code componentek
- Power Apps Component Framework (PCF) vezérlők írása

### PL-600: Microsoft Power Platform Solution Architect

Architekt szint: a komplex vállalati megoldások teljes tervezéséért felel, irányítja a fejlesztést, felügyeli a governancet, a megfelelőséget és külső rendszerek integrációját.
</details>

<details>
    <summary>
        <h2> MS Learn transcriptem</h2>
    </summary>

> Elméleti tudás validáció

[Microsoft Learn Transcript link - Szőke Péter](https://learn.microsoft.com/en-gb/users/speti/transcript/7k2lzf94gq2z9gl)

</details>

<details>
    <summary>
        <h2> Hands-on tapasztalatok</h2>
    </summary>

### Jira Logger

[Main flow link](https://make.powerautomate.com/environments/Default-e630c74d-c398-49fa-a067-c561ab5e8096/solutions/~preferred/flows/96728491-0213-ef11-9f89-000d3ab81244/details)

[Source code](samples/jira-logger-solution/src/Workflows/)

### Számlaleadás app

[App link](https://make.powerapps.com/environments/Default-e630c74d-c398-49fa-a067-c561ab5e8096/apps/3917f23d-9be1-4b99-97d4-7bfb0d584bb1/details)

[Source code](samples/szamlaleadas/src/Src/)

### Egyéb minták - nem prod

[samples/other/](samples/other/)

</details>

## Pilot projekt: XFlower újragondolva Power Platformon

[/pilot-project.md](/pilot-project.md)

[^1]: Kép hivatkozás [GemRain – Microsoft Power Platform Certification Roadmap](https://www.gemrain.net/post/microsoft-power-platform-certification-roadmap)
