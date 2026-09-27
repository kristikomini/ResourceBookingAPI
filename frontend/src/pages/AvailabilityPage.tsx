import { useState, type FormEvent } from "react";
import { useMutation } from "@tanstack/react-query";
import { searchAvailability } from "../api/endpoints";
import type { PaginatedResult, Resource } from "../api/types";

export default function AvailabilityPage() {
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [error, setError] = useState<string | null>(null);

  const search = useMutation<PaginatedResult<Resource>, unknown, void>({
    mutationFn: () =>
      searchAvailability({
        dataInizio: `${start}:00`,
        dataFine: `${end}:00`,
        page: 1,
        pageSize: 20,
      }),
    onError: () => setError("Search failed. Check the date range."),
  });

  function handleSearch(e: FormEvent) {
    e.preventDefault();
    setError(null);
    if (!start || !end) return;
    if (new Date(start) >= new Date(end)) {
      setError("Start must be before end.");
      return;
    }
    search.mutate();
  }

  const result = search.data;

  return (
    <div>
      <header className="page-header">
        <h1>Find availability</h1>
      </header>

      <div className="card" style={{ marginBottom: "1.5rem" }}>
        <form className="inline-form" onSubmit={handleSearch}>
          <label>
            From
            <input
              type="datetime-local"
              value={start}
              onChange={(e) => setStart(e.target.value)}
              required
            />
          </label>
          <label>
            To
            <input
              type="datetime-local"
              value={end}
              onChange={(e) => setEnd(e.target.value)}
              required
            />
          </label>
          <button className="btn-primary" type="submit" disabled={search.isPending}>
            {search.isPending ? "Searching…" : "Search"}
          </button>
        </form>
        {error && <p className="error">{error}</p>}
      </div>

      {result && (
        <div className="card">
          <p className="muted" style={{ marginTop: 0 }}>
            {result.totalResults} resource(s) available in this window.
          </p>
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Type</th>
              </tr>
            </thead>
            <tbody>
              {result.results.map((r) => (
                <tr key={r.resourceId}>
                  <td>{r.resourceId}</td>
                  <td>{r.name}</td>
                  <td>
                    <span className="badge">{r.resourceTypeName}</span>
                  </td>
                </tr>
              ))}
              {result.results.length === 0 && (
                <tr>
                  <td colSpan={3} className="muted">
                    Nothing available in this window.
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
