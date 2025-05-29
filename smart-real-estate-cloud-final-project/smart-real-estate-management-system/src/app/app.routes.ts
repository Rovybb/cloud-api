import { Routes } from '@angular/router';
import { PropertyListComponent } from './components/property-list/property-list.component';
import { PropertyCreateComponent } from './components/property-create/property-create.component';
import { PropertyUpdateComponent } from './components/property-update/property-update.component';
import { LoginComponent } from './components/identity/login/login.component';
import { RegisterComponent } from './components/identity/register/register.component';
import { HomePageComponent } from './pages/home-page/home-page.component';
import { ProfilePageComponent } from './pages/profile-page/profile-page.component';
import { AuthGuard } from './auth.guard';
import { PropertyDetailComponent } from './components/property-detail/property-detail.component';


export const appRoutes: Routes = [
  { path: '', component: HomePageComponent, pathMatch: 'full' },
  { path: 'home', redirectTo: '' },
  { path: 'properties', component: PropertyListComponent, canActivate: [AuthGuard] },
  { path: 'properties/create', component: PropertyCreateComponent, canActivate: [AuthGuard] },
  { path: 'properties/update/:id', component: PropertyUpdateComponent, canActivate: [AuthGuard] },
  { path: 'properties/property-details/:id', component: PropertyDetailComponent, canActivate: [AuthGuard] },
  { path: 'auth/login', component: LoginComponent },
  { path: 'auth/register', component: RegisterComponent },
  { path: 'profile', component: ProfilePageComponent, canActivate: [AuthGuard] }
];
