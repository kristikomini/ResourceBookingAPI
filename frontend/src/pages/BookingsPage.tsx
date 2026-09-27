import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createBooking,
  deleteBooking,
  getBookings,
  getResources,
} from "../api/endpoints";
import { useAuth } from "../auth/AuthContext";

function toIso(local: string) {
  // datetime-local gives "2026-09-26T14:30"; send it as-is (local time) plus seconds.
  return local.length === 16 ? `${local}:00` : local;
}

export default function BookingsPage() {
  const { user } = useAuth();
  const queryClient = useQueryClient();

  const bookings = useQuery({ queryKey: ["bookings"], queryFn: getBookings });
  const resources = useQuery({ queryKey: ["resources"], queryFn: getResources });

  const [resourceId, setResourceId] = useState<number | "">("");
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [formError, setFormError] = useState<string | null>(null);

  const resourceName = (id: number) =>
    resources.data?.find((r) => r.resourceId === id)?.name ?? `#${id}`;

  const create = useMutation({
    mutationFn: createBooking,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["bookings"] });
      setResourceId("");
      setStart("");
      setEnd("");
      setFormError(null);
    },
    onError: (err: unknown) => {
      const message =
        (err as { response?: { data?: string } })?.response?.data ??
        "Could not create booking (the resource may already be booked).";
      setFormError(typeof message === "string" ? message : "Booking failed.");
    },
  });

  const remove = useMutation({
    mutationFn: deleteBooking,
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["bookings"] }),
  });

  function handleCreate(e: FormEvent) {
    e.preventDefault();
    if (resourceId === "" || !start || !end || !user) return;
    if (new Date(start) >= new Date(end)) {
      setFormError("Start must be before end.");
      return;
    }
    create.mutate({
      resourceId: Number(resourceId),
      userId: user.userId,
      dataInizio: toIso(start),
      dataFine: toIso(end),
    });
  }

  return (
    <div>
      <header className="page-header">
        <h1>Bookings</h1>
      </header>

      <div className="card" style={{ marginBottom: "1.5rem" }}>
        <h2>New booking</h2>
        <form className="inline-form" onSubmit={handleCreate}>
          <label>
            Resource
            <select
              value={resourceId}
              onChange={(e) =>
                setResourceId(e.target.value ? Number(e.target.value) : "")
              }
              required
            >
              <option value="">Select…</option>
              {resources.data?.map((r) => (
                <option key={r.resourceId} value={r.resourceId}>
                  {r.name} ({r.resourceTypeName})
                </option>
              ))}
            </select>
          </label>
          <label>
            Start
            <input
              type="datetime-local"
              value={start}
              onChange={(e) => setStart(e.target.value)}
              required
            />
          </label>
          <label>
            End
            <input
              type="datetime-local"
              value={end}
              onChange={(e) => setEnd(e.target.value)}
              required
            />
          </label>
          <button className="btn-primary" type="submit" disabled={create.isPending}>
            {create.isPending ? "Booking…" : "Book"}
          </button>
        </form>
        {formError && <p className="error">{formError}</p>}
      </div>

      {bookings.isLoading && <p className="muted">Loading bookings…</p>}
      {bookings.isError && <p className="error">Could not load bookings.</p>}

      {bookings.data && (
        <div className="card">
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Resource</th>
                <th>From</th>
                <th>To</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {bookings.data.map((b) => (
                <tr key={b.bookingId}>
                  <td>{b.bookingId}</td>
                  <td>{resourceName(b.resourceId)}</td>
                  <td>{new Date(b.dataInizio).toLocaleString()}</td>
                  <td>{new Date(b.dataFine).toLocaleString()}</td>
                  <td>
                    <button
                      className="btn-ghost danger"
                      onClick={() => remove.mutate(b.bookingId)}
                      disabled={remove.isPending}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
              {bookings.data.length === 0 && (
                <tr>
                  <td colSpan={5} className="muted">
                    No bookings yet.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
