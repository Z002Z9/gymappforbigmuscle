import axiosInstance from "./axios.config.ts";


const Auth = {
    login: (email: string, password: string) => axiosInstance.post<{token: string}>(`login`, {email, password})
}


const api = {Auth};

export default api;