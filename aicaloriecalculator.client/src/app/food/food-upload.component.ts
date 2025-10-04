import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-food-upload',
  template: `
    <h4 style="color: #6f42c1;">Take or Upload Food Photo</h4>
    <input type="file" accept="image/*" capture="environment" class="form-control mb-3" (change)="onFileSelected($event)">
    <div *ngIf="imageUrl" class="mt-3">
      <img [src]="imageUrl" alt="Food" class="img-fluid rounded shadow" style="max-height: 300px;">
    </div>
  `
})
export class FoodUploadComponent {
  imageUrl: string | ArrayBuffer | null = null;
  @Output() imageChanged = new EventEmitter<string | ArrayBuffer | null>();

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      if (!file.type.startsWith('image/')) {
        alert('Please upload a valid image file.');
        return;
      }
      const reader = new FileReader();
      reader.onload = (e) => {
        this.imageUrl = reader.result;
        this.imageChanged.emit(this.imageUrl);
      };
      reader.readAsDataURL(file);
    }
  }
}
