import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginComponent } from '../login.component';
import { SignupComponent } from '../signup.component';
import { AuthService } from '../auth.service';
import { AuthRoutingModule } from './auth-routing.module';

@NgModule({
  declarations: [LoginComponent, SignupComponent],
  imports: [CommonModule, FormsModule, AuthRoutingModule],
  providers: [AuthService],
})
export class AuthModule {}
