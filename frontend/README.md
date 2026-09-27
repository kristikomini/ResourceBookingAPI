# ResourceBooking — Frontend

React + Vite + TypeScript client for the ResourceBooking .NET API.

## Stack

- **Vite** + **React 19** + **TypeScript**
- **React Router** — routing / protected routes
- **TanStack Query** — server state (fetching, caching, mutations)
- **Axios** — HTTP client with a JWT interceptor

## Prerequisites

Run the API first (from the repo root), using the **http** profile so there is no
HTTPS-redirect getting in the way of browser calls:

```bash
dotnet run --launch-profile http
```

The API listens on `http://localhost:5204`. If you seed the DB, log in with the
seed user **john.doe@example.com / password123**.

> The API must be restarted after the CORS change added to `Program.cs`,
> otherwise the browser will block requests.

## Run the frontend

```bash
cd frontend
npm install
npm run dev
```

Open http://localhost:5173.

## Configuration

The API base URL is read from `.env`:

```
VITE_API_URL=http://localhost:5204
```

## Structure

```
src/
  api/          Axios client, endpoint functions, DTO types
  auth/         AuthContext (JWT decode + login/logout)
  components/   Layout, ProtectedRoute
  pages/        Login, Availability, Resources, Bookings
```
