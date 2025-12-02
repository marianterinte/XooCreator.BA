# Story Evaluation System - Questions for Clarification

## 1. Session ID Management

**Question:** Când și cum generăm session ID-ul pentru o sesiune de citire?

**Options:**
- A) La începutul poveștii (când se încarcă prima pagină)
- B) La primul răspuns la quiz
- C) La primul apel API (submit quiz answer)

**Question:** Ce se întâmplă dacă utilizatorul închide browserul și revine?

**Options:**
- A) Generăm session ID nou (fiecare citire = sesiune nouă)
- B) Folosim același session ID (persistăm în localStorage)
- C) Permitem utilizatorului să continue sesiunea anterioară sau să înceapă una nouă

**Recomandare:** Generăm session ID la începutul poveștii și îl stocăm în localStorage pentru persistență.

---

## 2. Feedback la Răspunsuri

**Question:** Când arătăm feedback-ul pentru răspunsuri?

**Options:**
- A) Imediat după ce utilizatorul selectează un răspuns (corect/greșit vizibil instant)
- B) Doar la final, în ecranul de rezultate
- C) Opțional/configurabil per poveste

**Question:** Dacă arătăm feedback imediat, permitem utilizatorului să schimbe răspunsul?

**Options:**
- A) Da, permite schimbarea până când confirmă răspunsul
- B) Nu, răspunsul este blocat după prima selecție
- C) Depinde de setări (configurabil)

**Recomandare:** Feedback imediat + permite schimbarea până la confirmare.

---

## 3. Completare Parțială

**Question:** Ce facem dacă utilizatorul nu răspunde la toate quiz-urile?

**Options:**
- A) Calculăm scorul doar pentru quiz-urile la care a răspuns
- B) Cerem completarea tuturor quiz-urilor înainte de a calcula scorul
- C) Calculăm scorul, dar afișăm și numărul de quiz-uri nerezolvate

**Recomandare:** Calculăm scorul pentru quiz-urile răspuns, dar afișăm clar câte quiz-uri au fost omise.

---

## 4. Retry Behavior

**Question:** Când utilizatorul face retry, ce informații ar trebui să vadă?

**Options:**
- A) Nu arată nimic din încercarea anterioară (începe de la zero, fără influențe)
- B) Arată răspunsurile corecte din încercarea anterioară (pentru învățare)
- C) Arată doar scorul anterior, dar nu răspunsurile

**Recomandare:** Nu arătăm răspunsurile anterioare pentru a nu influența noua încercare, dar afișăm scorul anterior pentru motivație.

---

## 5. Tokens și Evaluare

**Question:** Cum gestionăm tokens-urile pentru poveștile evaluative?

**Options:**
- A) Tokens se acordă pentru fiecare răspuns corect (ca acum) + bonus la final pentru scoruri mari
- B) Tokens doar la final, bazat pe scorul total (ex: 100% = tokens max, 50% = tokens jumătate)
- C) Tokens pentru răspunsuri corecte + bonus pentru best score

**Recomandare:** Păstrăm tokens pentru răspunsuri corecte (comportament actual) + bonus la final pentru scoruri mari (ex: 100% = bonus tokens).

---

## 6. Navigare în Timpul Quiz-urilor

**Question:** Ce facem când utilizatorul încearcă să navigheze în timpul unui quiz?

**Options:**
- A) Blocăm navigarea până când răspunde la quiz
- B) Permitem skip, dar quiz-ul nerezolvat nu se numără în scor (0/0 pentru acel quiz)
- C) Permitem skip, dar marcăm quiz-ul ca "omis" și îl numărăm ca greșit în scor

**Recomandare:** Permitem skip, dar quiz-urile nerezolvate nu se numără în scor (0/0 pentru acel quiz).

---

## 7. Validare în Editor

**Question:** Dacă o poveste are quiz-uri dar `isEvaluative = false`, ce facem?

**Options:**
- A) Permitem (quiz-urile există dar nu sunt evaluate)
- B) Avertizăm creatorul să activeze evaluarea dacă există quiz-uri
- C) Auto-activăm evaluarea dacă detectăm quiz-uri (cu opțiune de dezactivare)

**Recomandare:** Permitem, dar sugerăm creatorului să activeze evaluarea dacă există quiz-uri.

---

## 8. Afișare Rezultate

**Question:** Unde și cum afișăm ecranul de rezultate?

**Options:**
- A) Modal peste poveste (overlay)
- B) Pagină separată (navigare către results page)
- C) Secțiune în pagina poveștii (scroll down pentru rezultate)

**Recomandare:** Modal peste poveste, cu opțiuni de retry sau închidere.

---

## 9. Best Score Display

**Question:** Unde afișăm best score-ul utilizatorului?

**Options:**
- A) Doar în ecranul de rezultate
- B) În lista de povești (badge pentru poveștile completate cu best score)
- C) În detaliile poveștii (înainte de a o începe)
- D) Toate opțiunile de mai sus

**Recomandare:** În rezultate + badge în lista de povești pentru poveștile completate cu best score.

---

## 10. Story Versioning

**Question:** Dacă o poveste este modificată (quiz-uri adăugate/șterse) după ce un utilizator a început să o citească, ce versiune folosim pentru evaluare?

**Options:**
- A) Versiunea curentă la momentul completării (nu cea de la început)
- B) Versiunea de la momentul când a început să citească (snapshot)
- C) Versiunea curentă, dar avertizăm dacă structura s-a schimbat

**Recomandare:** Folosim versiunea curentă la momentul completării (nu cea de la început).

---

## 11. Multiple Attempts Tracking

**Question:** Cât de multe încercări permitem să fie stocate?

**Options:**
- A) Toate încercările (fără limită)
- B) Ultimele N încercări (ex: ultimele 10)
- C) Doar best score + ultima încercare

**Recomandare:** Toate încercările (fără limită) pentru istoric complet.

---

## 12. Quiz Answer Overwrite

**Question:** Dacă utilizatorul răspunde la același quiz de două ori în aceeași sesiune, ce facem?

**Options:**
- A) Permitem overwrite (actualizăm răspunsul existent)
- B) Prevenim re-answering (UI disabled după primul răspuns)
- C) Stocăm ambele răspunsuri (timestamp diferit)

**Recomandare:** Permitem overwrite (actualizăm răspunsul existent) până când utilizatorul confirmă răspunsul.

---

## 13. Integration cu Existing Progress

**Question:** Cum se integrează sistemul de evaluare cu `UserStoryReadProgress` existent?

**Options:**
- A) Sistem separat, nu interferează
- B) `UserStoryReadProgress` marchează și quiz-urile ca "read" când sunt răspuns
- C) Quiz-urile nu se marchează ca "read" până când sunt răspuns corect

**Recomandare:** Sistem separat, dar quiz-urile răspuns se marchează ca "read" în `UserStoryReadProgress`.

---

## 14. Score Calculation Formula

**Question:** Cum calculăm exact scorul?

**Options:**
- A) Simplu: `(CorrectAnswers / TotalQuizzes) * 100`
- B) Weighted: Quiz-urile pot avea greutăți diferite
- C) Bonus/Penalty: Bonus pentru răspunsuri rapide, penalty pentru greșeli multiple

**Recomandare:** Simplu pentru MVP: `(CorrectAnswers / TotalQuizzes) * 100`. Weighted scoring poate fi adăugat mai târziu.

---

## 15. Results Screen Content

**Question:** Ce informații ar trebui să conțină ecranul de rezultate?

**Options:**
- A) Doar scorul total (percentage)
- B) Scorul + breakdown per quiz (care au fost corecte/greșite)
- C) Scorul + breakdown + timpul petrecut
- D) Scorul + breakdown + timpul + recomandări pentru îmbunătățire

**Recomandare:** Pentru MVP: Scorul total + breakdown per quiz. Detalii suplimentare pot fi adăugate mai târziu.

---

## Next Steps

După ce răspunzi la aceste întrebări, voi actualiza `STORY_EVALUATION_SYSTEM.md` cu deciziile tale și voi continua implementarea.

