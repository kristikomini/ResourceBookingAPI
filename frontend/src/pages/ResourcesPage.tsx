import { useQuery } from "@tanstack/react-query";
import { getResources } from "../api/endpoints";

export default function ResourcesPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["resources"],
    queryFn: getResources,
  });

  return (
    <div>
      <header className="page-header">
        <h1>Resources</h1>
      </header>

      {isLoading && <p className="muted">Loading resources…</p>}
      {isError && <p className="error">Could not load resources.</p>}

      {data && (
        <div className="card">
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Type</th>
              </tr>
            </thead>
            <tbody>
              {data.map((r) => (
                <tr key={r.resourceId}>
                  <td>{r.resourceId}</td>
                  <td>{r.name}</td>
                  <td>
                    <span className="badge">{r.resourceTypeName}</span>
                  </td>
                </tr>
              ))}
              {data.length === 0 && (
                <tr>
                  <td colSpan={3} className="muted">
                    No resources yet.
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
