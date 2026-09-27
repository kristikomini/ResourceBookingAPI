import { api } from "./client";
import type {
  AvailabilityQuery,
  Booking,
  BookingForCreation,
  PaginatedResult,
  Resource,
  ResourceType,
  User,
} from "./types";

export async function login(email: string, password: string): Promise<string> {
  const { data } = await api.post<{ token: string }>("/api/Auth/login", {
    email,
    password,
  });
  return data.token;
}

export async function getResources(): Promise<Resource[]> {
  const { data } = await api.get<Resource[]>("/api/resources");
  return data;
}

export async function getResourceTypes(): Promise<ResourceType[]> {
  const { data } = await api.get<ResourceType[]>("/api/resourcetypes");
  return data;
}

export async function getBookings(): Promise<Booking[]> {
  const { data } = await api.get<Booking[]>("/api/bookings");
  return data;
}

export async function createBooking(
  booking: BookingForCreation
): Promise<Booking> {
  const { data } = await api.post<Booking>("/api/bookings", booking);
  return data;
}

export async function deleteBooking(id: number): Promise<void> {
  await api.delete(`/api/bookings/${id}`);
}

export async function searchAvailability(
  query: AvailabilityQuery
): Promise<PaginatedResult<Resource>> {
  const { data } = await api.get<PaginatedResult<Resource>>(
    "/api/bookings/availability",
    { params: query }
  );
  return data;
}

export async function getUsers(): Promise<User[]> {
  const { data } = await api.get<User[]>("/api/User");
  return data;
}
