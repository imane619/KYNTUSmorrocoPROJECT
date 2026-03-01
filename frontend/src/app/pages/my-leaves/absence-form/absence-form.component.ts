import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card'; 
import { provideNativeDateAdapter } from '@angular/material/core';
import { Router } from '@angular/router'; // <--- AJOUTÉ

@Component({
  selector: 'app-absence-form',
  standalone: true,
  providers: [provideNativeDateAdapter()], // <--- AJOUTÉ : Pour corriger l'erreur Datepicker
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, 
    MatFormFieldModule, MatInputModule, MatSelectModule, 
    MatDatepickerModule, MatButtonModule, MatCardModule
  ],
  templateUrl: './absence-form.component.html'
})
export class AbsenceFormComponent {
  private snackBar = inject(MatSnackBar);
  private router = inject(Router); // <--- AJOUTÉ : Pour la redirection après envoi

  // Simulation d'un service si vous n'avez pas encore AbsenceService
  // Dans le futur, remplacez par : private absenceService = inject(AbsenceService);
  private absenceService = {
    createRequest: (data: any) => ({
      subscribe: (obs: any) => {
        console.log("Envoi à l'API Absence:", data);
        obs.next(); // Simule un succès
      }
    })
  };
  
  userSolde = signal(30); 
  totalDays = signal(0);
  selectedType = signal('');

  absenceForm = new FormGroup({
    type: new FormControl('', [Validators.required]),
    startDate: new FormControl(null, [Validators.required]),
    endDate: new FormControl(null, [Validators.required])
  });

  onTypeChange(value: string) {
    this.selectedType.set(value);
    this.calculateDays();
  }

  calculateDays() {
    const start = this.absenceForm.value.startDate;
    const end = this.absenceForm.value.endDate;
    if (start && end) {
      const total = this.calculateBusinessDays(new Date(start), new Date(end));
      this.totalDays.set(total);
    }
  }

  calculateBusinessDays(start: Date, end: Date): number {
    if (!start || !end) return 0;
    let count = 0;
    let curDate = new Date(start.getTime());
    while (curDate <= end) {
      if (curDate.getDay() !== 0) count++; // Samedi inclus
      curDate.setDate(curDate.getDate() + 1);
    }
    return count;
  }

  isSoldeInsuffisant(): boolean {
    return this.selectedType() === 'Payé' && this.totalDays() > this.userSolde();
  }

  onSubmit() {
    if (this.absenceForm.valid && !this.isSoldeInsuffisant()) {
      const data = this.absenceForm.value;
      
      this.absenceService.createRequest(data).subscribe({
        next: () => {
          this.snackBar.open("Demande envoyée au Manager !", "OK", { duration: 3000 });
          this.router.navigate(['/my-leaves']); // Redirection vers la liste
        },
        error: (err: any) => console.error("Erreur d'envoi", err)
      });
    }
  }
}