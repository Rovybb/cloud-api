import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NavbarComponent } from './navbar.component';

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;

  beforeEach(async () => {
    // Deoarece componenta este standalone, o importăm direct în configurația modulului de test
    await TestBed.configureTestingModule({
      imports: [NavbarComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have isMenuOpen set to false initially', () => {
    expect(component.isMenuOpen).toBeFalse();
  });

  it('should toggle isMenuOpen when toggleMenu is called', () => {
    // Valoarea inițială trebuie să fie false
    expect(component.isMenuOpen).toBeFalse();

    // Apelăm toggleMenu pentru a schimba valoarea în true
    component.toggleMenu();
    expect(component.isMenuOpen).toBeTrue();

    // Apelăm toggleMenu din nou pentru a reveni la false
    component.toggleMenu();
    expect(component.isMenuOpen).toBeFalse();
  });
});
