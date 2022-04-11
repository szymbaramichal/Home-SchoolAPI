# Home-School

##### PL:
##### Aplikacja będąca platformą elearningową oferującą: możliwość kontroli nad swoją klasą, tworzenia sprawdzianów i zadań, dodawania ocen i wiele innych!
#### Ten projekt jest API aplikacji. 

#### Home-School zakłada istnienie trzech typów użytkowników:
* **Wychowawca** - Wychowawca ma dostęp do informacji o klasie, może tworzyć przedmioty i dodawać nauczycieli do klasy. Od niego zależy który z uczniów będzie należał do klasy.
* **Nauczyciel** - Nauczyciel w klasie ma dostęp tylko do informacji o swoim przedmiocie (chyba, że jest wychowawcą i nauczycielem w klasie jednocześnie). Może on tworzyć sprawdziany, zadania etc.
* **Uczeń** - Uczeń może należeć do jednej klasy. Ma dostęp do zadanych mu prac oraz czatu klasowego.

------------

### Technikalia:
#### Backend:
* Język: C# w technologii .NET CORE 3.1
* **Technologie wykorzystane i funkcjonalności**: protokół SMTP, tokeny webowe JWT, SHA512, DependencyInjection, Swashbuckle - Swagger, autorski system przesyłania plików.
* Baza danych: MongoDB 



##### EN:
##### The application is an e-learning platform that offers: the ability to control your class, create tests and tasks, add grades and many more!
#### This project is an API for application.

#### Home-School assumes three types of users:
* ** Class teacher ** - The class teacher has access to information about the class, can create subjects and add another teachers to the class. It is up to him which of the students will belong to the class.
* ** Teacher ** - Normal teacher in the class has access only to information about his subject (unless he is the class teacher and the teacher in the class at the same time). It can create tests, tasks etc.
* ** Student ** - A student can belong to one class. He has access to the jobs assigned to him and the class chat.

------------

### Technics:
#### Backend:
* Language: C# used in .NET CORE 3.1 framework.
* ** Technologies used and functionalities **: SMTP protocol, JWT web tokens, SHA512, DependencyInjection, Swashbuckle - Swagger, own file transfer system.
* Database: MongoDB
