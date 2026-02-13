# SharedTravelling

## Project Overview

This web application addresses the growing need for affordable and sustainable travel options by creating a platform where:

- **Drivers** can post available seats in their vehicles for specific routes
- **Passengers** can search and book seats for their desired destinations
- **Both** benefit from reduced travel costs and environmental impact

Built entirely in Bulgarian to serve the local market, the platform handles the complete lifecycle of shared trips from creation to completion.

---

## Key Features

### For Passengers
- **Browse & Search Trips** - Find rides by route, date, and price
- **Filter Results** - Narrow down options based on preferences
- **Reserve Seats** - Book available spots in upcoming trips
- **View Driver Profiles** - See ratings, car details, and trip history

### For Drivers
- **Create Trips** - Post available rides with detailed information
- **Live Preview** - See exactly how your trip posting will look while creating it *(a feature I'm particularly proud of)*
- **Track Bookings** - Monitor seat availability in real-time

### System Features
- **Role-Based Authentication** - Separate access levels for passengers, drivers, and admins
- **Background Services** - Automated trip scheduling and status updates
- **Localized Interface** - Full Bulgarian language support including Cyrillic usernames
- **Responsive Design** - Works across desktop and mobile devices
- **Admin Panel** - User and trip management with moderation tools

---

## Technologies Used

| Layer      | Technology                       |
|-----------|----------------------------------|
| Backend    | ASP.NET Core (.NET)              |
| Frontend   | HTML, CSS, JavaScript            |
| Database   | PostgreSQL (hosted on Neon)     |
| Deployment | Docker, Koyeb                    |
| ORM        | Entity Framework Core            |
| Auth       | ASP.NET Identity                 |
| Hosting    | Web App + API Endpoints          |

---

## What I Learned

This project pushed my skills in:

- Full-stack development using C# / .NET  
- Database design & scalability with PostgreSQL  
- Authentication & security using Identity  
- Async background services (scheduling + updates)  
- Container / cloud deployment pipelines  
- Responsive UI development  

---

## Challenges & Solutions

| Challenge                     | Solution                                      |
|-------------------------------|-----------------------------------------------|
| Handling real-time trip updates| Background hosted services in ASP.NET Core   |
| Scalable data relationships   | Normalized PostgreSQL schema and migrations |
| Deployment automation          | Environment variables + CI/CD via Koyeb      |

---

## Cool & Special Features

- Live Preview — see how trip posting will appear before submission  
- Background Scheduler — handles trip statuses asynchronously  
- Role Authentication — different experiences for drivers vs passengers  
- Admin Dashboard — manage platform content efficiently  

---

## Data Flow

```text
+----------------------+
|         UI           |  <-- Users interact via forms/buttons
+----------------------+
           |
           v
+----------------------+
| Web API Controllers  |
+----------------------+
           |
           v
+----------------------+
| Business Logic Layer |
| (Services + Auth)    |
+----------------------+
           |
           v
+----------------------+
|   EF Core / ORM      |
+----------------------+
           |
           v
+----------------------+
| PostgreSQL Database  |
+----------------------+
```

---

## Deployment Flow

```text
GitHub ── CI/CD ──> Koyeb ──> Container
                     |
                     └─> Reads ENV vars (DB connection, keys)
                            |
                            -> Connects to Neon PostgreSQL
```

---

## Features to Add (Future Improvements)

- Map integration for routes
- Real-time notifications
- Mobile app integration
- Social sharing / Invite links

---

## License

This project is open-source and free to use, modify, and learn from.

---

## Contribution

Contributions are welcome! Please open an issue or submit a pull request.

---

## Demo Users 

### Passengers

| Username      | Password                       |
|-----------|----------------------------------|
| ЕленаМаркова    | еленамаркова              |
| БоянПетров   | боянпетров            |
| СилвияНиколова   | силвияниколова    |
| ЛюбомирИлиев | любомирилиев                   |
| ДесиславаПопова     | десиславапопова      |

### Drivers

| Username      | Password                       |
|-----------|----------------------------------|
| ИванПетров    | иванпетров              |
| МарияИванова   | марияиванова            |
| ГеоргиДимитров   | георгидимитров    |
| ПетърСтоянов | петърстоянов                   |
| ДимитърАнгелов     | димитърангелов      |

### Admin

| Username      | Password                       |
|-----------|----------------------------------|
| Админ    | админ123              |
