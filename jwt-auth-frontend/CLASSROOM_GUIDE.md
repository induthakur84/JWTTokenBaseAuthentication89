# 🎓 Simple Angular JWT Practical Guide (Classroom Lecture Plan)

This project has been intentionally kept **ultra-simple with minimal CSS** so that you can explain it line-by-line to your students on a projector or whiteboard without any confusing styles or boilerplate getting in the way.

---

## 📌 The 4 Core Files to Show Your Students

### 1. The Interceptor (`src/app/interceptors/jwt.interceptor.ts`)
*Explain:* "An Interceptor is like middleware in Angular. Before any HTTP request leaves the browser, this code attaches the JWT token from `localStorage` into the `Authorization: Bearer <token>` header."

```typescript
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};
```

---

### 2. The Auth Guard (`src/app/guards/auth.guard.ts`)
*Explain:* "The Auth Guard protects our routes. If a student tries to navigate directly to `/products` without logging in, Angular blocks them and redirects them to `/login`."

```typescript
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};
```

---

### 3. The Login Service Method (`src/app/services/auth.service.ts`)
*Explain:* "When the user clicks Login, we send their credentials to `.NET`. Upon a successful response, we store the JWT string into `localStorage`."

```typescript
login(request: LoginRequest) {
  return this.http.post<LoginResponse>(`${this.apiUrl}/User/Login`, request).pipe(
    tap((response) => {
      localStorage.setItem('jwt_token', response.token);
      localStorage.setItem('current_user', JSON.stringify(response.user));
    })
  );
}
```

---

### 4. The Protected Screen (`src/app/components/products/`)
*Explain:* "This single screen demonstrates everything to the students:"
1. **The Stored Token**: A simple text box showing the active JWT token currently stored in `localStorage`.
2. **GET Request (`/api/Product/GetAll`)**:
   - If logged in as **Admin** or **Employee** ➡️ Returns **HTTP 200 OK** and displays the products table.
   - If logged in as **User** ➡️ Returns **HTTP 403 Forbidden** (because `.NET` requires the `AdminOnly` policy!).
3. **POST Request (`/api/Product/Add`)**:
   - Only works for **Admin** (`[Authorize(Roles = "Admin")]`).
4. **DELETE Request (`/api/Product/Delete`)**:
   - Only works for **Employee** (`[Authorize(Roles = "Employee")]`).

---

## 🏃‍♂️ How to Run During Class

1. **Start the .NET Web API** in Visual Studio (`Ctrl + F5`).
2. **Start Angular**:
   ```bash
   cd c:\YouTube\JwtTokenBaseAuthentication\jwt-auth-frontend
   npm start
   ```
3. Open `http://localhost:4200` in the browser.
4. Click the demo buttons (**Admin**, **Employee**, or **User**) to demonstrate role-based access to your students in seconds!
