import React, { useEffect, useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { Loader, Button } from "@mantine/core";


interface Exercise {
  id: number;
  name: string;
  mainmuscle: string;
  youtubelink?: string;
  setnumber: number;
  repnumber: number;
  affectedBodyParts: string[];
}

interface User {
  id: number;
  bannedExercises: string[];
  email?: string;
}

const Bannexercises: React.FC = () => {
  const { token, email } = useContext(AuthContext);
  const [exercises, setExercises] = useState<Exercise[]>([]);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [updating, setUpdating] = useState(false);

  const API_BASE = import.meta.env.VITE_API_BASE ?? "https://localhost:7226";

  useEffect(() => {
    const abort = new AbortController();
    async function load() {
      setLoading(true);
      setError(null);
      try {
        const resEx = await fetch(`${API_BASE}/api/exercise`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
          signal: abort.signal,
        });
        if (!resEx.ok) throw new Error(`Failed to load exercises`);
        const dataEx = await resEx.json();
        setExercises(dataEx);

        if (email && token) {
          const resUser = await fetch(`${API_BASE}/api/user/ListUserByEmail/${encodeURIComponent(email)}`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
            signal: abort.signal,
          });
          if (resUser.ok) {
            const dataUser = await resUser.json();
            setUser(dataUser);
          } else {
            console.warn("Failed to load user data:", resUser.status);
          }
        }
      } catch (err: any) {
        if (err.name !== "AbortError") setError(err.message || "Failed to load");
      } finally {
        setLoading(false);
      }
    }
    load();
    return () => abort.abort();
  }, [token, email]);

  const toggleBanned = async (exerciseName: string) => {
    if (!user || !token) return;

    setUpdating(true);
    const bannedList = Array.isArray((user as any).bannedexercises) 
      ? (user as any).bannedexercises 
      : [];
    const isBanned = bannedList.includes(exerciseName);
    const updated = isBanned
      ? bannedList.filter((e: string) => e !== exerciseName)
      : [...bannedList, exerciseName];

    try {
      
      const res = await fetch(`${API_BASE}/api/user/EditUserByID/${user.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          name: user.name || "",//itt hibát ír de így jó valamiért szóval ne nyúlj bele
          email: user.email || "",
          age: (user as any).age || 0,
          height: (user as any).height || 0,
          weight: (user as any).weight || 0,
          gender: (user as any).gender || "",
          goal: (user as any).goal || "",
          trainingtype: (user as any).trainingtype || "",
          trainingsperweek: (user as any).trainingsperweek || 0,
          kcalintake: (user as any).kcalintake || 0,
          injury: (user as any).injury || [],
          allergys: (user as any).allergys || [],
          bannedexercises: updated,
        }),
      });
      if (res.ok) {
        setUser({ ...user, bannedexercises: updated } as any);
        console.log(`${exerciseName} ${isBanned ? "removed from" : "added to"} banned exercises`);
      } else {
        setError("Failed to update banned exercises");
      }
    } catch (err: any) {
      setError(err.message || "Error updating");
    } finally {
      setUpdating(false);
    }
  };

  if (loading) return <div style={{ color: "#fff" }}><Loader /></div>;
  if (error) return <div style={{ color: "#fff" }}>Error: {error}</div>;

  return (
    <div style={{ color: "#fff", padding: 16 }}>
      <h2>Exercises</h2>
      <table style={{ width: "100%", borderCollapse: "collapse", color: "#fff" }}>
        <thead>
          <tr>
            <th style={{ textAlign: "left", borderBottom: "1px solid #444", padding: 8 }}>Gyakorlat neve</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #444", padding: 8 }}>Fő izomcsoport</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #444", padding: 8 }}>Sorozatok száma </th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #444", padding: 8 }}>Ismétlések száma</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #444", padding: 8 }}>Érintett testrészek</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #444", padding: 8 }}>YouTube link</th>
            <th style={{ textAlign: "left", borderBottom: "1px solid #444", padding: 8 }}>Tiltólistabeli állapot</th>
          </tr>
        </thead>
        <tbody>
          {exercises.map((ex) => {
            const bannedList = Array.isArray((user as any)?.bannedexercises)
              ? (user as any).bannedexercises
              : [];
            const isBanned = bannedList.includes(ex.name);
            return (
              <tr key={ex.id}>
                <td style={{ padding: 8, borderBottom: "1px solid #2a2a2a" }}>{ex.name}</td>
                <td style={{ padding: 8, borderBottom: "1px solid #2a2a2a" }}>{ex.mainmuscle}</td>
                <td style={{ padding: 8, borderBottom: "1px solid #2a2a2a" }}>{ex.setnumber}</td>
                <td style={{ padding: 8, borderBottom: "1px solid #2a2a2a" }}>{ex.repnumber}</td>
                <td style={{ padding: 8, borderBottom: "1px solid #2a2a2a" }}>{(ex.affectedBodyParts || []).join(", ")}</td>
                <td style={{ padding: 8, borderBottom: "1px solid #2a2a2a" }}>
                  {ex.youtubelink ? <a href={ex.youtubelink} target="_blank" rel="noreferrer">Link</a> : "-"}
                </td>
                <td style={{ padding: 8, borderBottom: "1px solid #2a2a2a" }}>
                  <Button
                    color={isBanned ? "red" : "green"}
                    size="xs"
                    onClick={() => toggleBanned(ex.name)}
                    disabled={updating}
                    loading={updating}
                  >
                    {isBanned ? "Tiltólistára rakás" : "Levétel a tiltólistáról"}
                  </Button>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};

export default Bannexercises;