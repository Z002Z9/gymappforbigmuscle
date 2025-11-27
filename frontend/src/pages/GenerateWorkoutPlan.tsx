import React, { useContext, useState } from "react";
import { AuthContext } from "../context/AuthContext";
import { Button, Loader } from "@mantine/core";

interface Exercise {
    name: string;
    mainMuscle: string;
    youtubeLink: string;
    setNumber: number;
    repNumber: number;
    affectedBodyParts: string[];
}

const GenerateWorkoutPlan: React.FC = () => {
    const { email, token } = useContext(AuthContext);
    const [hasTriedGenerate, setHasTriedGenerate] = useState(false);

    const [exercises, setExercises] = useState<Exercise[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const mapRawToExercise = (raw: any): Exercise => ({
        name: raw.name ?? "",
        mainMuscle: raw.mainmuscle ?? "",
        youtubeLink: raw.youtubelink ?? "",
        setNumber: Number(raw.setnumber ?? 0),
        repNumber: Number(raw.repnumber ?? 0),
        affectedBodyParts:
            Array.isArray(raw.affectedBodyParts) && raw.affectedBodyParts.length > 0
                ? raw.affectedBodyParts.map(String)
                : [],
    });

    const generateWorkout = async () => {
        setHasTriedGenerate(true);
        if (!token || !email) {
            setError("Be kell jelentkezned!");
            return;
        }

        setLoading(true);
        setError(null);
        try {
            const res = await fetch(`/api/generateworkout/workoutgenerator`, {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!res.ok) {
                const text = await res.text();
                throw new Error(`API hiba: ${res.status} ${text}`);
            }

            const data = await res.json();
            console.log("Workout API válasz:", data);

            const rawList: any[] = Array.isArray(data) ? data : data.exercises ?? [data];


            const mapped = rawList.slice(0, 6).map(mapRawToExercise);

            setExercises(mapped);
        } catch (err) {
            setError(err instanceof Error ? err.message : String(err));
            setExercises([]);
        } finally {
            setLoading(false);
        }
  
    };

    return (
        <div style={{ marginTop: 20 }}>
            <Button onClick={generateWorkout} disabled={loading || !token}>
                {loading ? <Loader size="xs" /> : "Edzésterv generálása"}
            </Button>            

            {error && <div style={{ color: "red", marginTop: 10 }}>{error}</div>}

            {exercises.length > 0 && (
                <table style={{
                    width: "100%",
                    marginTop: 20,
                    borderCollapse: "collapse",
                    backgroundColor: "#1e1e1e",
                    color: "#fff",
                    borderRadius: "8px",
                    overflow: "hidden"
                }}>
                
                    <thead style={{ backgroundColor: "#2a2a2a" }}>
                        <tr>
                            <th style={{ padding: 12, textAlign: "left" }}>Név</th>
                            <th style={{ padding: 12, textAlign: "left" }}>Fő izom</th>
                            <th style={{ padding: 12, textAlign: "center" }}>Körök</th>
                            <th style={{ padding: 12, textAlign: "center" }}>Ismétlések</th>
                            <th style={{ padding: 12, textAlign: "left" }}>Videó</th>
                            <th style={{ padding: 12, textAlign: "left" }}>Érintett izmok</th>
                        </tr>
                    </thead>
                    <tbody>
                        {exercises.map((ex, idx) => (
                            <tr key={idx} style={{ borderBottom: "1px solid #333" }}>
                                <td style={{ padding: 10 }}>{ex.name}</td>
                                <td style={{ padding: 10 }}>{ex.mainMuscle}</td>
                                <td style={{ padding: 10, textAlign: "center" }}>{ex.setNumber}</td>
                                <td style={{ padding: 10, textAlign: "center" }}>{ex.repNumber}</td>
                                <td style={{ padding: 10 }}>
                                    {ex.youtubeLink ? (
                                        <a href={ex.youtubeLink} target="_blank" rel="noopener noreferrer" style={{ color: "#4ea1f3" }}>
                                            YouTube
                                        </a>
                                    ) : "-"}
                                </td>
                                <td style={{ padding: 10 }}>{ex.affectedBodyParts.join(", ")}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>

            )}

            {!loading && hasTriedGenerate && exercises.length === 0 && !error && (
                <div
                    style={{
                        marginTop: 10,
                        padding: "12px 16px",
                        backgroundColor: "#1e1e1e",
                        color: "#fff",
                        borderRadius: "8px",
                        textAlign: "center",
                        fontStyle: "italic",
                        border: "1px solid #333",
                    }}
                >
                    Nincs megjeleníthető edzésterv.
                </div>
            )}

        </div>
    );
};

export default GenerateWorkoutPlan;
