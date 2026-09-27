// TypeScript mirrors of the API DTOs (see /Dto in the backend project).

export interface ResourceType {
  resourceTypeId: number;
  typeName: string;
}

export interface Resource {
  resourceId: number;
  name: string;
  resourceTypeId: number;
  resourceTypeName: string;
}

export interface Booking {
  bookingId: number;
  resourceId: number;
  userId: number;
  dataInizio: string; // ISO date-time
  dataFine: string; // ISO date-time
}

export interface BookingForCreation {
  resourceId: number;
  userId: number;
  dataInizio: string;
  dataFine: string;
}

export interface User {
  userId: number;
  email: string;
  name: string;
  lastName: string;
}

export interface PaginatedResult<T> {
  totalResults: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
  results: T[];
}

export interface AvailabilityQuery {
  dataInizio: string;
  dataFine: string;
  codiceRisorsa?: number;
  page: number;
  pageSize: number;
}
