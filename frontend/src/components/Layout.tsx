import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function Layout() {
  const { user, logout } = useAuth();

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">📅 ResourceBooking</div>
        <nav>
          <NavLink to="/availability">Find availability</NavLink>
          <NavLink to="/resources">Resources</NavLink>
          <NavLink to="/bookings">Bookings</NavLink>
        </nav>
        <div className="sidebar-footer">
          <div className="user-name">{user?.name || user?.email}</div>
          <button className="btn-ghost" onClick={logout}>
            Log out
          </button>
        </div>
      </aside>
      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}
