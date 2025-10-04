import { Component } from '@angular/core';
import { FoodService } from './food/food.service';

@Component({
  selector: 'app-food-photo',
  templateUrl: './food-photo.component.html',
  styleUrls: ['./food-photo.component.css']
})
export class FoodPhotoComponent {
  imageUrl: string | ArrayBuffer | null = null;
  result: string = '';
  loading: boolean = false;

  constructor(private foodService: FoodService) {}

  onImageChanged(image: string | ArrayBuffer | null) {
    this.imageUrl = image;
    if (image) {
      this.loading = true;
      this.foodService.analyzeImage(image).subscribe(res => {
        this.result = res;
        this.loading = false;
      }, err => {
        this.result = 'Error analyzing image.';
        this.loading = false;
      });
    } else {
      this.result = '';
    }
  }
}
