import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FoodPhotoComponent } from '../food-photo.component';
import { FoodUploadComponent } from './food-upload.component';
import { FoodResultComponent } from './food-result.component';
import { FoodService } from './food.service';
import { FoodRoutingModule } from './food-routing.module';

@NgModule({
  declarations: [FoodPhotoComponent, FoodUploadComponent, FoodResultComponent],
  imports: [CommonModule, FormsModule, FoodRoutingModule],
  providers: [FoodService],
  exports: [FoodPhotoComponent, FoodUploadComponent, FoodResultComponent],
})
export class FoodModule {}
