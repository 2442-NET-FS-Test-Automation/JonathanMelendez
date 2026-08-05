import { NavLink, Route, Routes } from 'react-router-dom'
import './App.css'

import CatalogPage from './pages/CatalogPage'
import BookDetailPage from './pages/BookDetailPage'
import AboutPage from './pages/AboutPage'
import LoginPage from './pages/LoginPage'
import { RequireAuth } from './components/RequireAuth'
import AdminPage from './pages/AdminPage'
import { useAuth } from './ctx/AuthCtx'

function App() {
    const { status, user, logout } = useAuth()

  return (
      <div className='app'>
          <header className='app-header'>
            <h1>Library</h1>
            <nav className='app-header'>
                <NavLink to="/">Catalog</NavLink>
                <NavLink to="/about">About</NavLink>
                {/* Only admins can see the admin link */}
                {user?.role === "admin" && <NavLink to="/admin">Admin</NavLink>}
            </nav>

        <div className='auth-box'>
            {status === "authenticated" ? (
            <>
                <span>
                    {user?.name} ({user?.role})
                </span>
                <button type='button' onClick={logout}>
                    Sign out
                </button>
            </>
            ) : (
            <NavLink to="/login">Sign in</NavLink>
            )}
        </div>
      </header>
          <main>
                <Routes>
                    <Route path='/' element={<CatalogPage />}/>
                    <Route path='/inventory/:sku' element={<BookDetailPage />}/>
                    <Route path='/login' element={<LoginPage />}/>
                    <Route path='/about' element={<AboutPage />}/>

                    <Route element={<RequireAuth children={undefined} />}>

                    </Route>
                    <Route path="/admin" element={<AdminPage />} />
                    <Route path="*" element={<p>Page non fount</p>}/>
                </Routes>
          </main>
      </div>
  )
}

export default App
