# 📁 NUEVA ESTRUCTURA ORGANIZACIONAL - Frontend por Roles

## 🎯 ORGANIZACIÓN POR CARACTERÍSTICAS (FEATURES)

```
MyCleanApp.Client/
├── Features/
│   ├── Authentication/          # 🔐 Autenticación
│   │   ├── Pages/
│   │   │   ├── Login.razor
│   │   │   └── Register.razor
│   │   └── Services/
│   │       └── Auth.cs
│   │
│   ├── Docente/                # 👨‍🏫 Rol Docente
│   │   ├── Pages/
│   │   │   ├── TeacherDashboard.razor
│   │   │   ├── SubirCapacitaciones.razor
│   │   │   ├── SubirProyectoInvestigacion.razor
│   │   │   └── SubirPublicaciones.razor
│   │   ├── Components/
│   │   │   ├── TeacherDashboardEvaluacionesMud.razor
│   │   │   ├── TeacherDashboardPapers.razor
│   │   │   ├── TeacherDashboardPapersMud.razor
│   │   │   ├── TeacherDashboardRequirements.razor
│   │   │   ├── TeacherDashboardResearch.razor
│   │   │   ├── TeacherDashboardTraining.razor
│   │   │   └── TeacherApelaciones.razor
│   │   └── Services/
│   │       ├── DocenteService.cs
│   │       └── EvaluacionService.cs
│   │
│   ├── Administrador/          # 👨‍💼 Rol Administrador
│   │   ├── Pages/
│   │   │   └── AdminDashboard.razor
│   │   └── Components/
│   │       ├── AdminDashboardEvaluations.razor
│   │       ├── AdminDashboardHeader.razor
│   │       ├── AdminDashboardPromotions.razor
│   │       ├── AdminDashboardPromotions_new.razor
│   │       ├── AdminDashboardReports.razor
│   │       ├── AdminDashboardStats.razor
│   │       ├── AdminDashboardTeachers.razor
│   │       ├── AdminPromocionesMetodologia.razor
│   │       ├── AdminRequirementsTable.razor
│   │       └── GestionSolicitudesPromocion.razor
│   │
│   ├── ComisionAcademica/      # 🏛️ Comisión Académica
│   │   ├── Pages/
│   │   │   ├── ComisionPresidenteDashboard.razor
│   │   │   └── ComisionMiembroDashboard.razor
│   │   └── Components/
│   │       ├── ComisionAcademicaInfo.razor
│   │       ├── ComisionAcademicaPanel.razor
│   │       └── SistemaApelaciones.razor
│   │
│   ├── TalentoHumano/          # 👥 Talento Humano
│   │   ├── Pages/
│   │   │   └── TalentoHumanoDashboard.razor
│   │   ├── Components/
│   │   │   └── ListaVerificacionComponent.razor
│   │   └── Services/
│   │       └── VerificacionService.cs
│   │
│   ├── ConsejoUniversitario/   # 🎓 Consejo Universitario
│   │   ├── Pages/
│   │   │   └── ConsejoUniversitarioDashboard.razor
│   │   └── Components/
│   │       └── (componentes específicos del consejo)
│   │
│   └── Shared/                 # 🤝 Componentes y Servicios Compartidos
│       ├── Components/
│       │   ├── Home.razor
│       │   ├── RequisitoPromocion.razor
│       │   └── RequisitoPromocionMud.razor
│       └── Services/
│           ├── LocalStorageService.cs
│           └── SolicitudPromocionService.cs
│
├── Layout/                     # 📐 Layouts Globales
├── DTOs/                       # 📊 Data Transfer Objects
├── wwwroot/                    # 🌐 Assets Estáticos
└── App.razor                   # 🚀 Componente Raíz
```

## 🔧 BENEFICIOS DE LA NUEVA ESTRUCTURA

### ✅ Organización por Roles
- **Separación clara** de funcionalidades por tipo de usuario
- **Fácil mantenimiento** de código específico por rol
- **Escalabilidad** para agregar nuevos roles

### ✅ Principio de Responsabilidad Única
- Cada carpeta contiene solo lo relacionado con su rol
- Servicios específicos agrupados con sus componentes
- Componentes compartidos en carpeta dedicada

### ✅ Navegación Mejorada
- **Desarrollo más rápido** - encontrar archivos por rol
- **Onboarding simplificado** para nuevos desarrolladores
- **Debugging enfocado** por área funcional

## 🚀 PRÓXIMOS PASOS

1. **Actualizar rutas** en archivos de configuración
2. **Verificar imports** en todos los componentes
3. **Probar compilación** y funcionamiento
4. **Documentar patrones** de cada rol

## 📋 CONVENCIONES DE NOMBRES

- **Pages/**: Componentes que son páginas completas (tienen @page)
- **Components/**: Componentes reutilizables específicos del rol
- **Services/**: Servicios de datos específicos del rol
- **Shared/**: Todo lo que se comparte entre múltiples roles

## ⚠️ CONSIDERACIONES IMPORTANTES

- **Imports**: Actualizar `_Imports.razor` con nuevas rutas
- **Program.cs**: Registrar servicios desde nuevas ubicaciones
- **Rutas**: Verificar que todas las rutas @page sigan funcionando
- **Referencias**: Actualizar referencias entre componentes
