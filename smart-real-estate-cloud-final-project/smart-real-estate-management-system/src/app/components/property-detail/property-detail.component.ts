import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { Property } from '../../models/property.model'; // Import modelul Property
import { CommonModule } from '@angular/common';
import { LoginService } from '../../services/identity/login.service'; // Import LoginService
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { NavbarHomeComponent } from '../navbar-home/navbar-home.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowUpFromBracket } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-property-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarHomeComponent, FontAwesomeModule],
  templateUrl: './property-detail.component.html',
  styleUrls: ['./property-detail.component.css']
})
export class PropertyDetailComponent implements OnInit {
  property: Property | null = null;
  errorMessage: string | null = null;
  faArrowUpFromBracket = faArrowUpFromBracket;

  images: string[] = []; // Variabilă pentru a stoca imaginile proprietății
  uploading: boolean = false; // Indicator pentru încărcarea imaginilor

  constructor(
    private route: ActivatedRoute,
    private propertyService: PropertyService,
    private router: Router,
    private loginService: LoginService, // Adaugă LoginService
    private cdr: ChangeDetectorRef
  ) {}

  recommendedPrice: number | null = null; // Store the recommended price
  loggedInUserId: string | null = null; // Store the logged-in user's ID


  ngOnInit(): void {
    const propertyId = this.route.snapshot.paramMap.get('id');
    this.loggedInUserId = this.loginService.getUserId(); // Get the logged-in user's ID
  
    if (propertyId) {
      this.propertyService.getPropertyById(propertyId).subscribe({
        next: (data) => {
          this.property = data;
          this.images = data.imageUrls || [];
          this.fetchRecommendedPrice(); // Fetch the recommended price after property data is loaded
        },
        error: (error) => {
          this.errorMessage = 'Error fetching property details. Please try again.';
          console.error('Fetch error:', error);
          this.router.navigate(['/properties']);
        },
      });
    } else {
      this.errorMessage = 'Property ID is missing!';
      this.router.navigate(['/properties']);
    }
  
    // Check payment status
    const queryParams = new URLSearchParams(window.location.search);
    const paymentStatus = queryParams.get('paymentStatus');
    if (paymentStatus === 'success') {
      this.paymentSuccess = true;
    } else if (paymentStatus === 'cancel') {
      alert('Payment was cancelled.');
    }
  }

fetchRecommendedPrice(): void {
  if (!this.property) {
    console.error('Property is null or undefined. Cannot fetch recommended price.');
    return;
  }

  const body = {
    price: this.property.price ?? 0,
    city: this.property.address ?? '',
    location: 'string',
    roomsNr: this.property.rooms ?? 0,
    surface: this.property.area ?? 0,
  };

  console.log('Request body for prediction:', body);

  this.propertyService.predictPrice(body).subscribe({
    next: (response) => {
      console.log('Recommended price response:', response);

      const predictedPrice = response;
      if (typeof predictedPrice !== 'number') {
        console.error('Invalid predicted price in response.');
        return;
      }

      // Ensure recommendedPrice is always >= property.price
      this.recommendedPrice =
        this.property?.price && this.property.price > predictedPrice
          ? this.property.price
          : predictedPrice;

      console.log('Updated recommended price:', this.recommendedPrice);

      // Trigger UI update
      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Error fetching recommended price:', error);
      this.recommendedPrice = null;
    },
  });
}



  onImageUpload(event: Event): void {
    const propertyId = this.property?.id;
    if (!propertyId) {
      alert('Property ID is missing!');
      return;
    }
  
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      const formData = new FormData();
      formData.append('files', file); // Cheia corectă este 'files'
  
      this.uploading = true; // Setează indicatorul de upload
      this.propertyService.uploadImage(propertyId, formData).subscribe({
        next: (response) => {
          console.log('Upload successful:', response);
          if (response && response[0]?.url) {
            const imageUrl = response[0].url; // Extrage URL-ul imaginii din răspuns
            this.images.push(imageUrl); // Adaugă URL-ul în lista de imagini
          }
          this.uploading = false;
  
          // Resetează input-ul de fișiere
          input.value = ''; // Golește valoarea input-ului pentru a permite încărcări succesive
          alert('Image uploaded successfully!');
        },
        error: (error) => {
          this.uploading = false;
          console.error('Error uploading image:', error);
          alert('Failed to upload image. Please check the file and try again.');
        },
      });
    } else {
      alert('No file selected!');
    }
  }

  zoomedImage: string | null = null; // Variabilă pentru imaginea mărită

  openZoomModal(image: string): void {
    this.zoomedImage = image; // Setează imaginea care va fi afișată în modal
  }

  closeZoomModal(): void {
    this.zoomedImage = null; // Resetează imaginea pentru a închide modalul
  }
  



  isInquiryPopupOpen: boolean = false; // Control pentru deschiderea modalului
  inquiryMessage: string = ''; // Mesajul trimis în Inquiry

  openInquiryPopup(): void {
    this.isInquiryPopupOpen = true; // Deschide modalul
  }

  closeInquiryPopup(): void {
    this.isInquiryPopupOpen = false; // Închide modalul
    this.inquiryMessage = ''; // Resetează mesajul
  }

  sendInquiry(): void {
    if (!this.inquiryMessage.trim()) {
      alert('Please write a message before sending.');
      return;
    }

  const clientId = this.loginService.getUserId(); // Obține ID-ul utilizatorului
  if (!clientId) {
    alert('You need to be logged in to send an inquiry.');
    return;
  }

  if (this.property && this.property.id && this.property.userId) {
    const agentId = this.property.userId; // Proprietarul proprietății
    const status = 0; // Status implicit

    this.propertyService
      .sendInquiry(this.property.id, this.inquiryMessage, status, agentId, clientId)
      .subscribe({
        next: () => {
          alert('Your inquiry has been sent successfully!');
          this.closeInquiryPopup(); // Închide modalul
        },
        error: (err) => {
          console.error('Error sending inquiry:', err);
          alert('Failed to send inquiry. Please try again.');
        },
      });
  } else {
    alert('Property information is missing. Unable to send inquiry.');
  }
}

  // Other Methods (unchanged)
  goBack(): void {
    this.router.navigate(['/properties']);
  }

  navigateToUpdate(property: Property) {
    this.router.navigate(['properties/update/' + property.id]);
  }

  deleteProperty(property: Property): void {
    if (confirm('Are you sure you want to delete this property?')) {
      this.propertyService.deleteProperty(property.id).subscribe({
        next: () => {
          this.router.navigate(['/properties']);
        },
        error: (err) => {
          console.error('Error deleting property:', err);
          alert('Failed to delete property.');
        }
      });
    }
  }


  getPropertyStatus(status: any): string {
    switch (status) {
      case 0:
        return 'AVAILABLE';
      case 1:
        return 'SOLD';
      case 2:
        return 'RENTED';
      default:
        return 'UNKNOWN';
    }
  }

  getPropertyType(type: any): string {
    switch (type) {
      case 0:
        return 'HOUSE';
      case 1:
        return 'APARTMENT';
      case 2:
        return 'LAND';
      case 3:
        return 'COMMERCIAL';
      default:
        return 'UNKNOWN';
    }
  } 
  
  paymentSuccess: boolean = false; // Control pentru modalul de succes

  closePaymentSuccessModal(): void {
    this.paymentSuccess = false; // Închide modalul
  }

  makePayment(): void {
    if (!this.property || !this.property.id || !this.property.userId) {
      alert('Property or owner information is missing!');
      return;
    }

    const buyerId = this.loginService.getUserId();
    if (!buyerId) {
      alert('You need to be logged in to make a payment.');
      return;
    }

    const currentUrl = window.location.href; // URL-ul curent
    const successUrl = `${currentUrl}?paymentStatus=success`;
    const cancelUrl = `${currentUrl}?paymentStatus=cancel`;

    const body = {
      type: 0,
      price: this.property.price,
      date: new Date().toISOString(),
      status: 1,
      paymentMethod: 0,
      propertyId: this.property.id,
      sellerId: this.property.userId,
      buyerId: buyerId,
      successUrl: successUrl,
      cancelUrl: cancelUrl,
    };

    this.propertyService.createCheckoutSession(body).subscribe({
      next: (response) => {
        if (response?.checkoutUrl) {
          window.location.href = response.checkoutUrl; // Redirecționează către checkout
        } else {
          alert('Failed to create checkout session.');
        }
      },
      error: (err) => {
        console.error('Error creating checkout session:', err);
        alert('Failed to create checkout session. Please try again.');
      },
    });
  }


}