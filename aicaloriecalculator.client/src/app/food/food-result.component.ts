import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-food-result',
  template: `
    <div class="bg-white p-4 rounded shadow">
      <h5 class="mb-3" style="color: #6f42c1;">AI Analysis</h5>
      <div *ngIf="result; else noResult">
        <pre>{{ result }}</pre>
      </div>
      <ng-template #noResult>
        <p class="text-muted">Upload or take a photo to see calorie and food info.</p>
      </ng-template>
    </div>
  `
})
export class FoodResultComponent {
  @Input() result: string = '';
}
