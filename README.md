# Home-School

##### Aplikacja będąca platformą elearningową oferującą: możliwość kontroli nad swoją klasą, tworzenia sprawdzianów i zadań, dodawania ocen i wiele innych!
#### Ten projekt jest API aplikacji. 

#### Home-School zakłada istnienie trzech typów użytkowników:
* **Wychowawca** - Wychowawca ma dostęp do informacji o klasie, może tworzyć przedmioty i dodawać nauczycieli do klasy. Od niego zależy który z uczniów będzie należał do klasy.
* **Nauczyciel** - Nauczyciel w klasie ma dostęp tylko do informacji o swoim przedmiocie (chyba, że jest wychowawcą i nauczycielem w klasie jednocześnie). Może on tworzyć sprawdziany, zadania etc.
* **Uczeń** - Uczeń może należeć do jednej klasy. Ma dostęp do zadanych mu prac oraz czatu klasowego.

------------

### Technikalia:
#### Backend:
* **Język: C# w technologii .NET CORE 3.1
* **Technologie wykorzystane i funkcjonalności**: protokół SMTP, tokeny webowe JWT, SHA512, DependencyInjection, Swashbuckle - Swagger, autorski system przesyłania plików.
* **Baza danych: MongoDB 
