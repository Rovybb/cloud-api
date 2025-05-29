import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ProfileComponent } from './profile.component';
import { NavbarHomeComponent } from '../navbar-home/navbar-home.component';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileComponent, HttpClientTestingModule], // Adăugăm HttpClientTestingModule
      declarations: [NavbarHomeComponent], // Dacă NavbarHomeComponent nu este standalone, trebuie declarat
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // Inițializează componenta și apelează ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
