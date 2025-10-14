import {useContext, useEffect} from "react";
import {AuthContext} from "../context/AuthContext.tsx";
import {emailKeyName, emailTokenKey, tokenKeyName} from "../constants/constants.ts";
import {jwtDecode, JwtPayload} from "jwt-decode";
import api from "../api/api.ts";

interface CustomJwtPayload extends JwtPayload {
    [key: string]: any; 
}

const useAuth = () => {
    const { token, setToken, email, setEmail  } = useContext(AuthContext);
    const isLoggedIn = !!token;

    const login = (email: string, password: string) => {
        console.log({email, password});
        api.Auth.login(email, password).then(res => {
            const tokenFromBE = res.data.token;
            setToken(tokenFromBE); localStorage.setItem(tokenKeyName, tokenFromBE);
            setEmail(email); localStorage.setItem(emailKeyName, email);            
        },error => {
            alert("Helytelen jelszó vagy email!");
        });
        
    }

    const logout = () => {
        localStorage.clear();
        setToken(null);
    }

    const loginKata = (token: string) => {
        setToken(token); localStorage.setItem(tokenKeyName, token);
        const decodedToken = jwtDecode<CustomJwtPayload>(token);
        const tempEmail = decodedToken[emailTokenKey];
        localStorage.setItem(emailKeyName, tempEmail); setEmail(tempEmail);
    }

    useEffect(() => {

    }, []);

    return {login, logout, loginKata, token, email, isLoggedIn};
}

export default useAuth;