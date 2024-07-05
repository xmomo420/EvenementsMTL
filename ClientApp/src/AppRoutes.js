import { Login } from "./components/Login";
import { Inscription } from "./components/Inscription";
import { Home } from "./components/Home";
import { Evenement } from "./components/Evenement";
import { Recherche } from "./components/Recherche";
import { Profil } from "./components/Profil";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/login',
    element: <Login />
  },
  {
    path: '/inscription',
    element: <Inscription />
  },
  {
    path: '/evenement/:id',
    element: <Evenement />
  },
  {
    path: '/recherche',
    element: <Recherche />
  },
  {
    path: '/profil',
    element: <Profil/>
  }
];

export default AppRoutes;
