import Login from "../pages/Login.tsx";
import Dashboard from "../pages/Dashboard.tsx";
import Kcalcalculator from "../pages/Kcalcalculator.tsx";
import EditProfile from "../pages/EditProfile.tsx";
import Registrate from "../pages/Registrate.tsx";
export const routes = [
    {
        path: "login",
        component: <Login/>,
        isPrivate: false
    },
    {
        path: "dashboard",
        component: <Dashboard/>,
        isPrivate: true
    },
    {
        path: "kcalcalculator",
        component: <Kcalcalculator/>,
        isPrivate: true
    },
    {
        path: "profile",
        component: <EditProfile/>,
        isPrivate: true
    },
    {
        path: "registrate",
        component: <Registrate />,
        isPrivate: false
    }
]