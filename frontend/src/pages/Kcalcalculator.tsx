import { useEffect } from "react";
import axiosInstance from "../api/axios.config";

const Kcalcalculator = () => {

    useEffect(() => {
        axiosInstance.get("exercise").then(res => {
            console.log(res.data);        
    })
    }, []);

    return (
        <div>
            Kcalcalculator
        </div>
    )
}

export default Kcalcalculator;
