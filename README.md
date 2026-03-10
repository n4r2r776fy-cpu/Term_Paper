<<<<<<< HEAD
classDiagram
    %% СТИЛІЗАЦІЯ
    style Domain_Entities fill:#f9f9f9,stroke:#333
    style IRepository fill:#e1f5fe,stroke:#01579b
    style Notification fill:#f1f8e9,stroke:#33691e

    %% 1. DOMAIN LAYER (Інкапсуляція та SRP)
    namespace Domain_Entities {
        class Person {
            <<abstract>>
            -int id
            -string name
            -string phone
            -string email
            +getId() int
        }

        class Client {
            -Date dateOfBirth
            +getDateOfBirth() Date
        }

        class Specialist {
            -string specialization
            -int experience
            +getSpecialization() string
        }

        class Appointment {
            -int appointmentId
            -Date date
            -Time time
            -string status
            +getStatus() string
            +setStatus(string status)
        }

        class Service {
            -int serviceId
            -string name
            -double price
            -int duration
        }
    }

    %% 2. SERVICES (Виправлення SRP та Observer Coupling)
    namespace Services {
        class AuthService {
            +login(Person p)
            +logout(Person p)
        }
        class AppointmentService {
            -IRepository repo
            -Subject notificationManager
            +createAppointment(data)
            +cancelAppointment(id)
        }
    }

    %% 3. REPOSITORY PATTERN (DIP - Interface Inversion)
    namespace Repositories {
        class IRepository {
            <<interface>>
            +getAll()
            +getById(id)
            +save(entity)
            +delete(id)
        }
        class ClientRepository {
            +getAll()
            +save(client)
        }
        class AppointmentRepository {
            +getAll()
            +save(appointment)
        }
    }

    %% 4. OBSERVER & NOTIFICATIONS (OCP - Open/Closed Principle)
    namespace Notification_System {
        class Observer {
            <<interface>>
            +update(string message)
        }
        class Subject {
            <<interface>>
            +attach(Observer o)
            +notify(string message)
        }
        class Notification {
            <<interface>>
            +send(string message)
        }
        class EmailNotification {
            +send(string message)
        }
        class SmsNotification {
            +send(string message)
        }
    }

    %% ЗВ’ЯЗКИ
    Person <|-- Client
    Person <|-- Specialist
    
    %% Реалізація інтерфейсів (DIP)
    IRepository <|.. ClientRepository
    IRepository <|.. AppointmentRepository
    
    %% Реалізація сповіщень (OCP)
    Notification <|.. EmailNotification
    Notification <|.. SmsNotification
    
    %% Сервіси керують логікою
    AppointmentService --> IRepository : uses
    AppointmentService --> Subject : triggers notify
    AuthService --> Person : authenticates
    
    %% Observer зв'язки
    Client ..|> Observer
    Subject --> Observer
    Observer --> Notification : uses polymorphic send()
=======
classDiagram
    %% СТИЛІЗАЦІЯ
    style Domain_Entities fill:#f9f9f9,stroke:#333
    style IRepository fill:#e1f5fe,stroke:#01579b
    style Notification fill:#f1f8e9,stroke:#33691e

    %% 1. DOMAIN LAYER (Інкапсуляція та SRP)
    namespace Domain_Entities {
        class Person {
            <<abstract>>
            -int id
            -string name
            -string phone
            -string email
            +getId() int
        }

        class Client {
            -Date dateOfBirth
            +getDateOfBirth() Date
        }

        class Specialist {
            -string specialization
            -int experience
            +getSpecialization() string
        }

        class Appointment {
            -int appointmentId
            -Date date
            -Time time
            -string status
            +getStatus() string
            +setStatus(string status)
        }

        class Service {
            -int serviceId
            -string name
            -double price
            -int duration
        }
    }

    %% 2. SERVICES (Виправлення SRP та Observer Coupling)
    namespace Services {
        class AuthService {
            +login(Person p)
            +logout(Person p)
        }
        class AppointmentService {
            -IRepository repo
            -Subject notificationManager
            +createAppointment(data)
            +cancelAppointment(id)
        }
    }

    %% 3. REPOSITORY PATTERN (DIP - Interface Inversion)
    namespace Repositories {
        class IRepository {
            <<interface>>
            +getAll()
            +getById(id)
            +save(entity)
            +delete(id)
        }
        class ClientRepository {
            +getAll()
            +save(client)
        }
        class AppointmentRepository {
            +getAll()
            +save(appointment)
        }
    }

    %% 4. OBSERVER & NOTIFICATIONS (OCP - Open/Closed Principle)
    namespace Notification_System {
        class Observer {
            <<interface>>
            +update(string message)
        }
        class Subject {
            <<interface>>
            +attach(Observer o)
            +notify(string message)
        }
        class Notification {
            <<interface>>
            +send(string message)
        }
        class EmailNotification {
            +send(string message)
        }
        class SmsNotification {
            +send(string message)
        }
    }

    %% ЗВ’ЯЗКИ
    Person <|-- Client
    Person <|-- Specialist
    
    %% Реалізація інтерфейсів (DIP)
    IRepository <|.. ClientRepository
    IRepository <|.. AppointmentRepository
    
    %% Реалізація сповіщень (OCP)
    Notification <|.. EmailNotification
    Notification <|.. SmsNotification
    
    %% Сервіси керують логікою
    AppointmentService --> IRepository : uses
    AppointmentService --> Subject : triggers notify
    AuthService --> Person : authenticates
    
    %% Observer зв'язки
    Client ..|> Observer
    Subject --> Observer
    Observer --> Notification : uses polymorphic send()
>>>>>>> 4897911be475cf0163df939b60435fdfa0f92dd3
