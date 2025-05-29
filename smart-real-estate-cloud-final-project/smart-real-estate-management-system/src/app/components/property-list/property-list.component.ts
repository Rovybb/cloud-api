import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import { Property } from '../../models/property.model';
import { PropertyService } from '../../services/property.service';
import { Router } from '@angular/router';
import { NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarHomeComponent } from "../navbar-home/navbar-home.component";
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft, faArrowRight, faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-property-list',
  standalone: true,
  imports: [NgFor, FormsModule, NavbarHomeComponent, FontAwesomeModule],
  templateUrl: './property-list.component.html',
  /* Ai grijă să folosești `styleUrls` în loc de `styleUrl`: */
  styleUrls: ['./property-list.component.css']
})
export class PropertyListComponent implements OnInit {
  properties: Property[] = [];
  pageNumber: number = 1;
  pageSize: number = 2;
  totalPages: number = 0;
  faArrowLeft = faArrowLeft;
  faArrowRight = faArrowRight;
  faMagnifyingGlass = faMagnifyingGlass;

  filters: { [key: string]: string | number | null } = {
    title: null,
    price_min: null,
    price_max: null,
    description: null
  };

  constructor(
    private propertyService: PropertyService, 
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProperties();
  }

  loadProperties(): void {
    // Prelucrăm filtrele, eliminându-le pe cele goale
    const processedFilters: { [key: string]: string } = {};
    for (const key in this.filters) {
      const value = this.filters[key];
      if (value !== null && value !== undefined && value !== '') {
        processedFilters[key] = value.toString();
      }
    }

    console.log('Filters sent to backend:', processedFilters); // Debugging

    this.propertyService
      .getPropertiesWithPagination(this.pageNumber, this.pageSize, processedFilters)
      .subscribe((data: any) => {
        this.properties = data.items;
        this.totalPages = data.totalPages;

        // 2. După ce ai obținut lista de proprietăți,
        //    pentru fiecare proprietate faci câte un request la getPropertyById
        this.properties.forEach((property) => {
          this.propertyService.getPropertyById(property.id).subscribe({
            next: (fullProperty) => {
              // `fullProperty.imageUrls` conține array-ul de imagini
              property.imageUrls = fullProperty.imageUrls ? fullProperty.imageUrls : [];
            },
            error: (err) => {
              console.error(
                `Eroare la preluarea imaginilor pentru proprietatea cu ID=${property.id}:`,
                err
              );
              // Poți afișa un mesaj de eroare sau să lași un array gol
              property.imageUrls = [];
            },
          });
        });
      });
  }

  navigateToCreate(): void {
    this.router.navigate(['properties/create']);
  }

  viewDetails(property: Property): void {
    this.router.navigate(['/properties/property-details', property.id]);
  }

  applyFilters(): void {
    console.log('Filters applied:', this.filters);
    this.pageNumber = 1;
    this.loadProperties();
  }

  resetFilters(): void {
    this.filters = {
      title: null,
      price_min: null,
      price_max: null,
      description: null
    };
    this.pageNumber = 1;
    this.loadProperties();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.loadProperties();
    }
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadProperties();
    }
  }

  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.loadProperties();
    }
  }

  getPropertyType(status: any): string {
    switch (status) {
      case 0:
        return 'house';
      case 1:
        return 'apartment';
      case 2:
        return 'land';
      case 3:
        return 'commercial';
      default:
        return 'unknown'; // În caz de status invalid
    }
  }
  
  getPropertyStatus(status: any): string {
    switch (status) {
      case 0:
        return 'available';
      case 1:
        return 'sold';
      case 2:
        return 'rented';
      default:
        return 'unknown'; // În caz de status invalid
    }
  }
  
}
