import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarHomeComponent } from '../navbar-home/navbar-home.component';
import { UserInformationService, UserInformation } from '../../services/user-information.service'; // Importăm serviciul și interfața

@Component({
  selector: 'app-profile',
  standalone: true, // Această componentă este standalone
  imports: [CommonModule, NavbarHomeComponent], // Importăm CommonModule pentru directivele built-in (ex: *ngIf)
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
})
export class ProfileComponent implements OnInit {
  // Variabilă care indică dacă se află în modul de editare
  editMode: boolean = false;

  // Modelul de date al utilizatorului
  user: UserInformation | null = null;

  constructor(private userInformationService: UserInformationService) {}

  ngOnInit(): void {
    const userId = '123e4567-e89b-12d3-a456-426614174000'; // Exemplu de ID

    // Facem cererea către API pentru a obține informațiile utilizatorului
    // this.userInformationService.getUserById(userId).subscribe({
    //   next: (data) => {
    //     this.user = data;
    //     console.log('Informații utilizator:', this.user);
    //   },
    //   error: (err) => {
    //     console.error('Eroare la preluarea informațiilor utilizatorului:', err);
    //   },
    // });
  }

  // Activează modul de editare
  enableEdit(): void {
    this.editMode = true;
  }

  // Dezactivează modul de editare și procesează datele actualizate
  submitProfile(): void {
    this.editMode = false;
    console.log('Profil actualizat:', this.user);
    // Aici se poate adăuga logica pentru a trimite datele actualizate către un API
  }
}
