import { NavLink, Route, Routes } from 'react-router-dom'
import './App.css'

import CatalogPage from './pages/CatalogPage'
import BookDetailPage from './pages/BookDetailPage'
import AboutPage from './pages/AboutPage'

function App() {

  return (
      <div className='app'>
          <header className='app-header'>
              <h1>Library</h1>
              <nav className='app-header'>
                <NavLink to="/">Catalog</NavLink>
                <NavLink to="/about">About</NavLink>
              </nav>
          </header>
          <main>
                <Routes>
                    <Route path='/' element={<CatalogPage />}/>
                    <Route path='/inventory/:sku' element={<BookDetailPage />}/>
                    <Route path='/about' element={<AboutPage />}/>
                        
                    <Route path="*" element={<p>Page non fount</p>}/>
                </Routes>
          </main>
      </div>
  )
}

export default App
