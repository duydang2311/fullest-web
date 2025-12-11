# fullest-web

A task management platform designed for public collaboration.

## Tech Stack

### API (`/api`)
The core backend service, providing robust APIs for data management and business logic. It follows a hybrid of Clean Architecture and Vertical Slice Architecture principles.

- **Language:** C#.
- **Framework:** .NET 9, ASP.NET Core.
- **API:** FastEndpoints.
- **Database:** PostgreSQL with Entity Framework Core.
- **Authentication:** JWT & Google OAuth.

### Frontend (`/www`)
The user-facing web application, built for dynamic and responsive user experiences.

- **Language:** TypeScript.
- **Framework:** SvelteKit.
- **Styling:** Tailwind CSS.
- **Build Tool:** Vite.

### Workers (`/workers-assets`)
Serverless functions and edge computing for asset handling.

- **Language:** TypeScript.
- **Platform:** Cloudflare Workers.
- **Storage:** Cloudflare R2 & KV.

## Contributing

Contributions are always welcome. Feel free to reach out if you have any questions.

Happy coding!
