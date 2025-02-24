import { Component, OnInit } from '@angular/core';
import { GoalService } from './goal.service';
import { AuthService } from '../../../public/auth/auth.service';

@Component({
  selector: 'app-goal',
  templateUrl: './goal.component.html',
  styleUrls: ['./goal.component.scss']
})
export class GoalComponent implements OnInit {
  goals: any[] = [];
  newGoal: any = {
    Descricao: '',
    ValorAlvo: 0,
    ValorAtual: 0,
    DataLimite: ''
  };
  userId: string = this.authService.getDecodedToken().nameid;

  constructor(private goalService: GoalService, private authService: AuthService) { }

  ngOnInit(): void {
    // this.loadGoals();
  }

  // loadGoals() {
  //   this.goalService.getGoals(this.userId).subscribe((res) => {
  //     this.goals = res.data;
  //   });
  // }

  // createGoal() {
  //   this.newGoal.UserId = this.userId;
  //   this.newGoal.StampEntity = this.generateUniqueId();
  //   this.goalService.createGoal(this.newGoal).subscribe((res) => {
  //     this.loadGoals();
  //     this.newGoal = { Descricao: '', ValorAlvo: 0, ValorAtual: 0, DataLimite: '' };
  //   });
  // }

  generateUniqueId(): string {
    return Math.random().toString(36).substring(2, 15);
  }
}
