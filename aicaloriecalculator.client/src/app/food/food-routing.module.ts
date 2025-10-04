import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FoodPhotoComponent } from '../food-photo.component';

const routes: Routes = [
  { path: '', component: FoodPhotoComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FoodRoutingModule {}
