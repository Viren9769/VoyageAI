/* ============================================================
DASHBOARD
============================================================ */

export interface DashboardData {

  welcome: WelcomeSection;

  stats: DashboardStat[];
  aiPlanner: AiPlanner;

}

/* ============================================================
WELCOME
============================================================ */

export interface WelcomeSection {

  greeting: string;

  userName: string;

  subtitle: string;

}

/* ============================================================
STATS CARD
============================================================ */

export interface DashboardStat {

  title: string;

  value: number | string;

  icon: string;

  change: number;

  trend: 'up' | 'down';

  color: string;

}
/* ============================================================
AI PLANNER
============================================================ */

export interface AiPlanner {

  title: string;

  subtitle: string;

  placeholder: string;

  buttonText: string;

}