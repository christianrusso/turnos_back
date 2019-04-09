using Microsoft.AspNetCore.Identity;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.Database.Model;
using SistemaTurnos.Database.ModelData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.Populator
{
    public class Populator : IPopulator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public Populator(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Populate()
        {
            // Recreo el schema de la base de datos
            Console.Write("Borrando la base de datos y todos sus datos\t");
            DropDatabaseSchema();
            Console.Write("OK\n");

            Console.Write("Creando shema de la base de datos vacio\t\t");
            CreateDatabaseSchema();
            Console.Write("OK\n");

            Console.WriteLine();
            Console.WriteLine("Comenzando a popular la base de datos...");

            // Creo roles de usuario
            Console.Write("Roles\t\t\t\t");
            CreateRoles();
            Console.Write("OK\n");


            // Creo ciudades

            CreateCity("25 de Mayo");
            CreateCity("3 de febrero");
            CreateCity("Aarón Castellanos");
            CreateCity("Acassuso");
            CreateCity("Aconquija");
            CreateCity("Agronomía");
            CreateCity("Almagro");
            CreateCity("Avellaneda");
            CreateCity("Balvanera");
            CreateCity("Bandera");
            CreateCity("Banfield");
            CreateCity("Baradero");
            CreateCity("Bariloche");
            CreateCity("Barracas");
            CreateCity("Beccar");
            CreateCity("Belgrano");
            CreateCity("Belgrano R");
            CreateCity("Berazategui");
            CreateCity("Boca");
            CreateCity("Boedo");
            CreateCity("Caballito");
            CreateCity("Campana");
            CreateCity("Carmen de Patagones");
            CreateCity("Caseros");
            CreateCity("Caseros");
            CreateCity("Castelar");
            CreateCity("Chacarita");
            CreateCity("Ciudadela");
            CreateCity("Cnel. Du Graty");
            CreateCity("Coghlan");
            CreateCity("Colegiales");
            CreateCity("Concordia");
            CreateCity("Córdoba Capital");
            CreateCity("Don Torcuato");
            CreateCity("Escobar");
            CreateCity("Esquel");
            CreateCity("Esther");
            CreateCity("Ezeiza");
            CreateCity("Florencio Varela");
            CreateCity("Flores");
            CreateCity("Floresta");
            CreateCity("Florida");
            CreateCity("Garín");
            CreateCity("Gerli");
            CreateCity("González Catán");
            CreateCity("Gral. Pacheco");
            CreateCity("Gral. Pueyrredón");
            CreateCity("Grand Bourg");
            CreateCity("Guillermo Enrique Hudson");
            CreateCity("Haedo");
            CreateCity("Hurlingham");
            CreateCity("Ituzaingó");
            CreateCity("La Falda");
            CreateCity("La Lucila");
            CreateCity("La Paternal");
            CreateCity("La Plata");
            CreateCity("La Tablada");
            CreateCity("Lanús");
            CreateCity("Las Cañitas");
            CreateCity("Liniers");
            CreateCity("Lisandro de la Torre");
            CreateCity("Lomas de Zamora");
            CreateCity("Malvinas Argentinas");
            CreateCity("Mar del Plata");
            CreateCity("Martínez");
            CreateCity("Mataderos");
            CreateCity("Mendoza Capital");
            CreateCity("Merlo");
            CreateCity("Microcentro");
            CreateCity("Monserrat");
            CreateCity("Monte Castro");
            CreateCity("Monte Grande");
            CreateCity("Moreno");
            CreateCity("Morón");
            CreateCity("Neuquen Capital");
            CreateCity("Nordelta");
            CreateCity("Núñez");
            CreateCity("Olivos");
            CreateCity("Palermo");
            CreateCity("Paraná");
            CreateCity("Parque Chacabuco");
            CreateCity("Parque Chas");
            CreateCity("Parque Patricios");
            CreateCity("Pilar");
            CreateCity("Pinamar");
            CreateCity("Posadas");
            CreateCity("Puerto Madero");
            CreateCity("Quilmes");
            CreateCity("Rafael Castillo");
            CreateCity("Ramos Mejía");
            CreateCity("Ranelagh");
            CreateCity("Recoleta");
            CreateCity("Remedios de Escalada");
            CreateCity("Retiro");
            CreateCity("Rosario");
            CreateCity("Río Cuarto");
            CreateCity("Saavedra");
            CreateCity("San Cristóbal");
            CreateCity("San Fernando");
            CreateCity("San Francisco Solano");
            CreateCity("San Isidro");
            CreateCity("San Juan");
            CreateCity("San Justo");
            CreateCity("San Martín");
            CreateCity("San Miguel");
            CreateCity("San Miguel de 25");
            CreateCity("San Nicolás");
            CreateCity("San Nicolás");
            CreateCity("San Telmo");
            CreateCity("Santa Fe Capital");
            CreateCity("Santiago del Estero");
            CreateCity("Sarandí");
            CreateCity("Sta. Rosa");
            CreateCity("Tandil");
            CreateCity("Tigre");
            CreateCity("Tortuguitas");
            CreateCity("Valentín Alsina");
            CreateCity("Venado Tuerto");
            CreateCity("Versalles");
            CreateCity("Vicente López");
            CreateCity("Villa Ballester");
            CreateCity("Villa Carlos Paz");
            CreateCity("Villa Constitución");
            CreateCity("Villa Crespo");
            CreateCity("Villa del Parque");
            CreateCity("Villa Devoto");
            CreateCity("Villa Luro");
            CreateCity("Villa Luzuriaga");
            CreateCity("Villa Pueyrredon");
            CreateCity("Villa Urquiza");
            CreateCity("Vélez Sársfield");

            var clinicBusiness = CreateBusinessType("Clinic");
            var hairdressingBusiness = CreateBusinessType("Hairdressing");
            var barbershopBusiness = CreateBusinessType("Barbershop");
            var estheticBusiness = CreateBusinessType("Esthetic");

            // Creo clinicas
            Console.Write("Clinicas\t\t\t");
            // email,  password,  name,  description,  city,  address,  latitude,  longitude
            var clinic1 = CreateHairdressingUser(clinicBusiness, "clinica1@asd.com", "clinica1@asd.com", "Clinica 1", "Clinica de Villa Bosch 1", "Villa Bosch", "Jose Maria Bosch 951", -34.5883457, -58.5732785, true, "2128552166781000", "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G");
            var clinic2 = CreateHairdressingUser(clinicBusiness, "clinica2@asd.com", "clinica2@asd.com", "Clinica 2", "Clinica de Moron 1", "Moron", "Yatay 600", -34.6548052, -58.6173822, true, "2128552166781000", "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G");
            var clinic3 = CreateHairdressingUser(clinicBusiness, "clinica3@asd.com", "clinica3@asd.com", "Clinica 3", "Clinica de Villa Bosch 2", "Villa Bosch", "Julio Besada 6300", -34.5873598, -58.5852697, true, "2128552166781000", "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G");
            var clinic4 = CreateHairdressingUser(clinicBusiness, "clinica4@asd.com", "clinica4@asd.com", "Clinica 4", "Clinica de prueba", "prueba", "prueba", -34.5873598, -58.5852697, true, "2128552166781000", "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G");
            Console.Write("OK\n");

            // Creo peluquerias
            Console.Write("Peluquerias\t\t\t");
            // email,  password,  name,  description,  city,  address,  latitude,  longitude
            var hairdressing1 = CreateHairdressingUser(hairdressingBusiness, "peluqueria1@asd.com", "peluqueria1@asd.com", "Peluqueria 1", "Peluqueria de Villa Bosch 1", "Villa Bosch", "Jose Maria Bosch 951", -34.5883457, -58.5732785, true, "2128552166781000", "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G");
            Console.Write("OK\n");

            // Creo barberias
            Console.Write("Barberias\t\t\t");
            // email,  password,  name,  description,  city,  address,  latitude,  longitude
            var babershop1 = CreateHairdressingUser(barbershopBusiness, "barberia1@asd.com", "barberia1@asd.com", "Barberia 1", "Barberia de Villa Bosch 1", "Villa Bosch", "Jose Maria Bosch 951", -34.5883457, -58.5732785, true, "2128552166781000", "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G");
            Console.Write("OK\n");

            // Creo esteticas
            Console.Write("Esteticas\t\t\t");
            // email,  password,  name,  description,  city,  address,  latitude,  longitude
            var esthetic1 = CreateHairdressingUser(estheticBusiness, "estetica1@asd.com", "estetica1@asd.com", "Estetica 1", "Estetica de Villa Bosch 1", "Villa Bosch", "Jose Maria Bosch 951", -34.5883457, -58.5732785, true, "2128552166781000", "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G");
            Console.Write("OK\n");

            // Creo empleados
            Console.Write("Empleados\t\t\t");
            var employee1 = CreateEmployee("empleado1@asd.com", "empleado1@asd.com", clinic1);
            var employee2 = CreateEmployee("empleado2@asd.com", "empleado2@asd.com", clinic1);
            Console.Write("OK\n");

            // Creo especialidades
            Console.Write("Especialidades\t\t\t");
            var specialtyData0 = CreateSpecialtyData("TEST", clinicBusiness);
            var specialtyData1 = CreateSpecialtyData("Servicios de Maquillaje", hairdressingBusiness);
            var specialtyData2 = CreateSpecialtyData("Servicios de Peluqueria", hairdressingBusiness);
            var specialtyData3 = CreateSpecialtyData("Peinados", hairdressingBusiness);
            var specialtyData4 = CreateSpecialtyData("TRATAMIENTS de Cabellos", hairdressingBusiness);
            var specialtyData5 = CreateSpecialtyData("Rituales/Lavados", hairdressingBusiness);
            var specialtyData6 = CreateSpecialtyData("coloracion", hairdressingBusiness);
            var specialtyData7 = CreateSpecialtyData("Manicuria/Pedicuria", hairdressingBusiness);
            var specialtyData8 = CreateSpecialtyData("Corte", hairdressingBusiness);
            var specialtyData9 = CreateSpecialtyData("Servivicios de Barberia", barbershopBusiness);
            var specialtyData10 = CreateSpecialtyData("Servivicios de Peluqueria", barbershopBusiness);
            var specialtyData11 = CreateSpecialtyData("Cosmiatría", estheticBusiness);
            var specialtyData12 = CreateSpecialtyData("Tratamientos Corporales", estheticBusiness);
            var specialtyData13 = CreateSpecialtyData("Tratamientos Faciales", estheticBusiness);
            var specialtyData14 = CreateSpecialtyData("Camas Solares", estheticBusiness);
            var specialtyData15 = CreateSpecialtyData("Masajes", estheticBusiness);
            var specialtyData16 = CreateSpecialtyData("Depilacion", estheticBusiness);
            var specialtyData17 = CreateSpecialtyData("Pestañas", estheticBusiness);
            var specialtyData18 = CreateSpecialtyData("Spa", estheticBusiness);
            var specialtyData19 = CreateSpecialtyData("Manos y Pies", estheticBusiness);
            var specialtyData20 = CreateSpecialtyData("Medicina Estetica", estheticBusiness);

            Console.Write("OK\n");

            // Creo subespecialidades
            Console.Write("Subespecialidades\t\t");

            //********************************************* SERVICIOS DE MAQUILLAJE

            var especialityTEST = CreateSpecialty(clinicBusiness, "TEST", clinic1);
            var subespecialityTEST = CreateSubspecialty("TEST", especialityTEST, 10, clinic1);
            var subspecialtyData1 = CreateSubspecialtyData("Make up social", specialtyData1);
            var subspecialtyData2 = CreateSubspecialtyData("Novia ceremonia: make up", specialtyData1);
            var subspecialtyData3 = CreateSubspecialtyData("Novia ceremonia: make up y peinado", specialtyData1);
            var subspecialtyData4 = CreateSubspecialtyData("Peinado y maquillaje social", specialtyData1);
            var subspecialtyData5 = CreateSubspecialtyData("Novia civil: make up", specialtyData1);
            var subspecialtyData6 = CreateSubspecialtyData("Novia civil: make up y peinado", specialtyData1);
            var subspecialtyData7 = CreateSubspecialtyData("Madrinas ceremonia: make up", specialtyData1);
            var subspecialtyData8 = CreateSubspecialtyData("Madrinas ceremonia: peinado", specialtyData1);
            var subspecialtyData9 = CreateSubspecialtyData("Madrinas ceremonia: make up y peinado", specialtyData1);
            var subspecialtyData10 = CreateSubspecialtyData("Madrinas civil: make up", specialtyData1);
            var subspecialtyData11 = CreateSubspecialtyData("Perfilado de Cejas", specialtyData1);
            var subspecialtyData12 = CreateSubspecialtyData("Maquillaje social", specialtyData1);
            var subspecialtyData13 = CreateSubspecialtyData("Maquillaje novias y 15 años", specialtyData1);

            //******************************************************SERVICIOS DE PELUQUERÍA
            var subspecialtyData14 = CreateSubspecialtyData("Peinado social", specialtyData2);
            var subspecialtyData15 = CreateSubspecialtyData("Novia ceremonia: peinado", specialtyData2);
            var subspecialtyData16 = CreateSubspecialtyData("Novia civil: peinado", specialtyData2);
            var subspecialtyData17 = CreateSubspecialtyData("Lavado moroccanoil", specialtyData2);
            var subspecialtyData18 = CreateSubspecialtyData("Lavado común", specialtyData2);
            var subspecialtyData19 = CreateSubspecialtyData("Madrinas civil: peinado", specialtyData2);
            var subspecialtyData20 = CreateSubspecialtyData("Clases de autopeinado", specialtyData2);


            //****************************************************** PEINADOS
            var subspecialtyData21 = CreateSubspecialtyData("peinados de novia", specialtyData3);
            var subspecialtyData22 = CreateSubspecialtyData("peinado de 15", specialtyData3);
            var subspecialtyData23 = CreateSubspecialtyData("Modelado", specialtyData3);
            var subspecialtyData24 = CreateSubspecialtyData("Peinado con ondas", specialtyData3);
            var subspecialtyData25 = CreateSubspecialtyData("peinado de fiesta", specialtyData3);
            var subspecialtyData26 = CreateSubspecialtyData("brushing", specialtyData3);

            //****************************************************** TRATAMIENTS DE CABELLOS
            var subspecialtyData27 = CreateSubspecialtyData("alisado brazilian", specialtyData4);
            var subspecialtyData28 = CreateSubspecialtyData("Alisados", specialtyData4);
            var subspecialtyData29 = CreateSubspecialtyData("alisado cristal", specialtyData4);
            var subspecialtyData30 = CreateSubspecialtyData("alisado evans", specialtyData4);
            var subspecialtyData31 = CreateSubspecialtyData("Ampolla", specialtyData4);
            var subspecialtyData32 = CreateSubspecialtyData("botox capilar", specialtyData4);
            var subspecialtyData33 = CreateSubspecialtyData("shock de keratina", specialtyData4);
            var subspecialtyData34 = CreateSubspecialtyData("Baño de crema", specialtyData4);

            //****************************************************** RITUALES/LAVADOS
            var subspecialtyData35 = CreateSubspecialtyData("ampollas", specialtyData5);
            var subspecialtyData36 = CreateSubspecialtyData("cauterización", specialtyData5);
            var subspecialtyData37 = CreateSubspecialtyData("tratamiento moroccanoil", specialtyData5);

            //****************************************************** COLORACIÓN
            var subspecialtyData38 = CreateSubspecialtyData("baño de luz", specialtyData6);
            var subspecialtyData39 = CreateSubspecialtyData("Desgaste de puntas", specialtyData6);
            var subspecialtyData40 = CreateSubspecialtyData("color desde raiz", specialtyData6);
            var subspecialtyData41 = CreateSubspecialtyData("color touch", specialtyData6);
            var subspecialtyData42 = CreateSubspecialtyData("desgaste", specialtyData6);
            var subspecialtyData43 = CreateSubspecialtyData("decoloracion", specialtyData6);
            var subspecialtyData44 = CreateSubspecialtyData("Color s/amoniaco", specialtyData6);
            var subspecialtyData45 = CreateSubspecialtyData("Color inoa", specialtyData6);
            var subspecialtyData46 = CreateSubspecialtyData("mechas", specialtyData6);
            var subspecialtyData47 = CreateSubspecialtyData("Iluminación", specialtyData6);
            var subspecialtyData48 = CreateSubspecialtyData("reflejos", specialtyData6);
            var subspecialtyData49 = CreateSubspecialtyData("color fantasia", specialtyData6);
            var subspecialtyData50 = CreateSubspecialtyData("balayage", specialtyData6);

            //****************************************************** MANICURÍA/PEDICURÍA
            var subspecialtyData51 = CreateSubspecialtyData("belleza de pies", specialtyData7);
            var subspecialtyData52 = CreateSubspecialtyData("belleza de manos", specialtyData7);
            var subspecialtyData53 = CreateSubspecialtyData("Manicuria con parafina", specialtyData7);
            var subspecialtyData54 = CreateSubspecialtyData("manos semipermanente", specialtyData7);
            var subspecialtyData55 = CreateSubspecialtyData("pedicuria semipermanente", specialtyData7);
            var subspecialtyData56 = CreateSubspecialtyData("Pedicuría con esmalte en gel semi", specialtyData7);
            var subspecialtyData57 = CreateSubspecialtyData("Belleza de pies con esmalte en gel semi", specialtyData7);
            var subspecialtyData58 = CreateSubspecialtyData("pedicuria", specialtyData7);

            //****************************************************** Corte 
            var subspecialtyData59 = CreateSubspecialtyData("Corte", specialtyData8);
            var subspecialtyData60 = CreateSubspecialtyData("Corte caballero", specialtyData8);
            var subspecialtyData61 = CreateSubspecialtyData("Corte niño", specialtyData8);


            //****************************************************** SERVICIOS DE BARBERÍA
            var subspecialtyData62 = CreateSubspecialtyData("Barba", specialtyData9);
            var subspecialtyData63 = CreateSubspecialtyData("Corte y barba", specialtyData9);
            var subspecialtyData64 = CreateSubspecialtyData("Teñido de barba", specialtyData9);

            //****************************************************** SERVICIOS DE PELUQUERÍA
            var subspecialtyData65 = CreateSubspecialtyData("Corte", specialtyData10);
            var subspecialtyData66 = CreateSubspecialtyData("Corte de niño", specialtyData10);
            var subspecialtyData67 = CreateSubspecialtyData("Corte jubilados", specialtyData10);
            var subspecialtyData68 = CreateSubspecialtyData("Lavado normal", specialtyData10);
            var subspecialtyData69 = CreateSubspecialtyData("Lavado con masajes y mascarilla", specialtyData10);
            var subspecialtyData70 = CreateSubspecialtyData("Alisado", specialtyData10);
            var subspecialtyData71 = CreateSubspecialtyData("Mascarilla", specialtyData10);
            var subspecialtyData72 = CreateSubspecialtyData("Iluminación", specialtyData10);
            var subspecialtyData73 = CreateSubspecialtyData("Balayage", specialtyData10);
            var subspecialtyData74 = CreateSubspecialtyData("Perfilado de cejas", specialtyData10);
            var subspecialtyData75 = CreateSubspecialtyData("Claritos", specialtyData10);





            //++++++++++++++++++++++++++++++++++++++++++ Cosmiatría
            var subspecialtyData76 = CreateSubspecialtyData("Limpieza Facial Profunda", specialtyData11);
            var subspecialtyData77 = CreateSubspecialtyData("Máscara de LED (Light Emitting Diodes).", specialtyData11);
            var subspecialtyData78 = CreateSubspecialtyData("Microdermoabrasión", specialtyData11);
            var subspecialtyData79 = CreateSubspecialtyData("Peeling", specialtyData11);
            var subspecialtyData80 = CreateSubspecialtyData("Drenaje Linfático manual facial.", specialtyData11);
            var subspecialtyData81 = CreateSubspecialtyData("Rubber mask", specialtyData11);
            var subspecialtyData82 = CreateSubspecialtyData("Masajes", specialtyData11);
            var subspecialtyData83 = CreateSubspecialtyData("Microdermoabrasión", specialtyData11);
            var subspecialtyData84 = CreateSubspecialtyData("Drenaje Linfático Manual", specialtyData11);
            var subspecialtyData85 = CreateSubspecialtyData("Radiofrecuencia", specialtyData11);
            var subspecialtyData86 = CreateSubspecialtyData("Electroestimulación", specialtyData11);
            var subspecialtyData87 = CreateSubspecialtyData("Presoterapia", specialtyData11);

            //++++++++++++++++++++++++++++++++++++++++++ TRATAMIENTOS CORPORALES
            var subspecialtyData88 = CreateSubspecialtyData("Tratamiento anticelulitico reductor", specialtyData12);
            var subspecialtyData89 = CreateSubspecialtyData("Tratamiento reafirmante anticelulitico", specialtyData12);
            var subspecialtyData90 = CreateSubspecialtyData("Masaje descontracturante de espalda", specialtyData12);
            var subspecialtyData91 = CreateSubspecialtyData("Limpieza de cutis", specialtyData12);
            var subspecialtyData92 = CreateSubspecialtyData("Peeling de espalda", specialtyData12);
            var subspecialtyData93 = CreateSubspecialtyData("Rellenos y toxinas botulínicas", specialtyData12);
            var subspecialtyData94 = CreateSubspecialtyData("Peeling", specialtyData12);
            var subspecialtyData95 = CreateSubspecialtyData("Despigmentante", specialtyData12);
            var subspecialtyData96 = CreateSubspecialtyData("Bomba de nutrición", specialtyData12);
            var subspecialtyData97 = CreateSubspecialtyData("Limpieza de cutis", specialtyData12);
            var subspecialtyData98 = CreateSubspecialtyData("Extracciones", specialtyData12);
            var subspecialtyData99 = CreateSubspecialtyData("Exfoliación Facial", specialtyData12);
            var subspecialtyData100 = CreateSubspecialtyData("Lifting sin cirugía", specialtyData12);
            var subspecialtyData101 = CreateSubspecialtyData("Rejuvenecedor", specialtyData12);
            var subspecialtyData102 = CreateSubspecialtyData("Radiofrecuencia", specialtyData12);
            var subspecialtyData103 = CreateSubspecialtyData("Masajes", specialtyData12);
            var subspecialtyData104 = CreateSubspecialtyData("Espuma celulitis control", specialtyData12);
            var subspecialtyData105 = CreateSubspecialtyData("Exfoliación Corpora", specialtyData12);
            var subspecialtyData106 = CreateSubspecialtyData("Anticelulitis", specialtyData12);
            var subspecialtyData107 = CreateSubspecialtyData("Radiofrecuencia/Ultracavitación", specialtyData12);
            var subspecialtyData108 = CreateSubspecialtyData("Gel térmico / criógeno", specialtyData12);
            var subspecialtyData109 = CreateSubspecialtyData("Vendas", specialtyData12);
            var subspecialtyData110 = CreateSubspecialtyData("Mesoterapia para celulitis", specialtyData12);
            var subspecialtyData111 = CreateSubspecialtyData("HImFU", specialtyData12);
            var subspecialtyData112 = CreateSubspecialtyData("Reducción de adiposidad localizada intensiva", specialtyData12);
            var subspecialtyData113 = CreateSubspecialtyData("Flaccidez", specialtyData12);
            var subspecialtyData114 = CreateSubspecialtyData("Celulitis fláccida o edematosa", specialtyData12);
            var subspecialtyData115 = CreateSubspecialtyData("Peeling corporal- blanqueamiento", specialtyData12);
            var subspecialtyData116 = CreateSubspecialtyData("Modelación corporal", specialtyData12);
            var subspecialtyData117 = CreateSubspecialtyData("Endormologie", specialtyData12);
            var subspecialtyData118 = CreateSubspecialtyData("Vacunterapia micro circulación infrarrojo.", specialtyData12);
            var subspecialtyData119 = CreateSubspecialtyData("Pressoterapia", specialtyData12);
            var subspecialtyData120 = CreateSubspecialtyData("Corrientes rusas ,australianas y tens", specialtyData12);
            var subspecialtyData122 = CreateSubspecialtyData("Radiogrecuencia", specialtyData12);
            var subspecialtyData123 = CreateSubspecialtyData("Ultracavitación de alta frecuencia", specialtyData12);
            var subspecialtyData124 = CreateSubspecialtyData("Mesoterapia virtual", specialtyData12);
            var subspecialtyData125 = CreateSubspecialtyData("Dermo Health", specialtyData12);
            var subspecialtyData126 = CreateSubspecialtyData("Crio Frecuencia", specialtyData12);
            var subspecialtyData127 = CreateSubspecialtyData("Ultracavitación de alta frecuencia", specialtyData12);
            var subspecialtyData128 = CreateSubspecialtyData("Dermo Health", specialtyData12);
            var subspecialtyData129 = CreateSubspecialtyData("Ultracavitacion", specialtyData12);
            var subspecialtyData130 = CreateSubspecialtyData("Tratamiento para celulitis", specialtyData12);
            var subspecialtyData131 = CreateSubspecialtyData("Tratamiento para estrias", specialtyData12);
            var subspecialtyData132 = CreateSubspecialtyData("Tratamiento para flacidez", specialtyData12);
            var subspecialtyData133 = CreateSubspecialtyData("Altafrecuencia", specialtyData12);
            var subspecialtyData134 = CreateSubspecialtyData("Piunta de diamantes microdermoabrasion", specialtyData12);
            var subspecialtyData135 = CreateSubspecialtyData("Limpieza corporal inicial. Pulido", specialtyData12);
            var subspecialtyData136 = CreateSubspecialtyData("Drenaje linfatico mecanico - presoterapia", specialtyData12);
            var subspecialtyData137 = CreateSubspecialtyData("Drenaje linfatico manual", specialtyData12);
            var subspecialtyData138 = CreateSubspecialtyData("Criolipotisis", specialtyData12);
            var subspecialtyData139 = CreateSubspecialtyData("Electroestimulacion", specialtyData12);
            var subspecialtyData140 = CreateSubspecialtyData("Electroeracion corporal", specialtyData12);
            var subspecialtyData141 = CreateSubspecialtyData("Venus legacy", specialtyData12);
            var subspecialtyData142 = CreateSubspecialtyData("Bronceado sano - sol pleno", specialtyData12);
            var subspecialtyData143 = CreateSubspecialtyData("Vendras frias", specialtyData12);
            var subspecialtyData144 = CreateSubspecialtyData("Maderoterapia", specialtyData12);
            var subspecialtyData145 = CreateSubspecialtyData("Hifi", specialtyData12);
            var subspecialtyData146 = CreateSubspecialtyData("Eternity", specialtyData12);
            var subspecialtyData147 = CreateSubspecialtyData("Electrodos - ondas rusas", specialtyData12);
            var subspecialtyData148 = CreateSubspecialtyData("Dermo Health", specialtyData12);
            var subspecialtyData149 = CreateSubspecialtyData("Electroterapia", specialtyData12);
            var subspecialtyData150 = CreateSubspecialtyData("Crio Frecuencia", specialtyData12);
            var subspecialtyData151 = CreateSubspecialtyData("Radiofrecuencia", specialtyData12);
            var subspecialtyData152 = CreateSubspecialtyData("Drenaje linfatico manual", specialtyData12);
            var subspecialtyData153 = CreateSubspecialtyData("Drenaje linfático manual + botas de presoterapia", specialtyData12);
            var subspecialtyData154 = CreateSubspecialtyData("Tratamiento reductor", specialtyData12);
            var subspecialtyData155 = CreateSubspecialtyData("Tratamiento integral", specialtyData12);
            var subspecialtyData156 = CreateSubspecialtyData("Ultrasonido", specialtyData12);
            var subspecialtyData157 = CreateSubspecialtyData("Criolipolisis plana", specialtyData12);
            var subspecialtyData158 = CreateSubspecialtyData("Ondas de choques", specialtyData12);
            var subspecialtyData159 = CreateSubspecialtyData("Termoterapia combianada", specialtyData12);
            var subspecialtyData160 = CreateSubspecialtyData("Ondas rusas /electrodos", specialtyData12);
            var subspecialtyData161 = CreateSubspecialtyData("Ultracavitación", specialtyData12);
            var subspecialtyData162 = CreateSubspecialtyData("Ozonoterapia", specialtyData12);
            var subspecialtyData163 = CreateSubspecialtyData("Presoterapia", specialtyData12);
            var subspecialtyData164 = CreateSubspecialtyData("Tratamiento para estrias", specialtyData12);
            var subspecialtyData165 = CreateSubspecialtyData("Radiofrecuencia", specialtyData12);



            //++++++++++++++++++++++++++++++++++++++++++ TRATAMIENTOS FACIALES
            var subspecialtyData166 = CreateSubspecialtyData("Tratamiento anti age", specialtyData13);
            var subspecialtyData167 = CreateSubspecialtyData("Tratamiento humectación", specialtyData13);
            var subspecialtyData168 = CreateSubspecialtyData("Peeling químico", specialtyData13);
            var subspecialtyData169 = CreateSubspecialtyData("punta de diamante", specialtyData13);
            var subspecialtyData170 = CreateSubspecialtyData("Tratamiento intensivo para acné con mácara led", specialtyData13);
            var subspecialtyData171 = CreateSubspecialtyData("Fototerapia con máscara led", specialtyData13);

            var subspecialtyData173 = CreateSubspecialtyData("Peeling con máscara led", specialtyData13);
            var subspecialtyData174 = CreateSubspecialtyData("higiene facial", specialtyData13);
            var subspecialtyData175 = CreateSubspecialtyData("Tratamiento para acné", specialtyData13);
            var subspecialtyData176 = CreateSubspecialtyData("Limpieza de cutis con punta de diamantes", specialtyData13);
            var subspecialtyData177 = CreateSubspecialtyData("Peeling", specialtyData13);
            var subspecialtyData178 = CreateSubspecialtyData("Radiofrecuencia facial", specialtyData13);
            var subspecialtyData179 = CreateSubspecialtyData("ultherapy", specialtyData13);
            var subspecialtyData180 = CreateSubspecialtyData("accent ultra", specialtyData13);
            var subspecialtyData181 = CreateSubspecialtyData("harmony", specialtyData13);
            var subspecialtyData182 = CreateSubspecialtyData("laser co2", specialtyData13);
            var subspecialtyData183 = CreateSubspecialtyData("infini", specialtyData13);
            var subspecialtyData184 = CreateSubspecialtyData("mesoterapia", specialtyData13);
            var subspecialtyData185 = CreateSubspecialtyData("venus legacy", specialtyData13);
            var subspecialtyData186 = CreateSubspecialtyData("terapia de quelaciones", specialtyData13);
            var subspecialtyData187 = CreateSubspecialtyData("bioplatia", specialtyData13);
            var subspecialtyData188 = CreateSubspecialtyData("shape", specialtyData13);
            var subspecialtyData189 = CreateSubspecialtyData("dermapen", specialtyData13);
            var subspecialtyData190 = CreateSubspecialtyData("endymed", specialtyData13);
            var subspecialtyData191 = CreateSubspecialtyData("rinoplatica sin cirugia", specialtyData13);
            var subspecialtyData192 = CreateSubspecialtyData("plasmarico", specialtyData13);
            var subspecialtyData193 = CreateSubspecialtyData("radiesse", specialtyData13);
            var subspecialtyData194 = CreateSubspecialtyData("rellenos de acido hialurinico", specialtyData13);
            var subspecialtyData195 = CreateSubspecialtyData("Crio Frecuencia", specialtyData13);
            var subspecialtyData196 = CreateSubspecialtyData("Dermo Leds", specialtyData13);
            var subspecialtyData197 = CreateSubspecialtyData("Microdermoabrasión", specialtyData13);
            var subspecialtyData198 = CreateSubspecialtyData("PEELING MECANICO Y QUIMICO", specialtyData13);
            var subspecialtyData199 = CreateSubspecialtyData("ROCEASEA", specialtyData13);
            var subspecialtyData200 = CreateSubspecialtyData("AGNE", specialtyData13);
            var subspecialtyData201 = CreateSubspecialtyData("LIFTING ( CRIO RADIOFRECUENCIA)", specialtyData13);
            var subspecialtyData202 = CreateSubspecialtyData("LIFTING ( HI FU ULTHERAPY)", specialtyData13);
            var subspecialtyData203 = CreateSubspecialtyData("MICRO DERMOHABRACION", specialtyData13);
            var subspecialtyData204 = CreateSubspecialtyData("TERAPIA LUMINICA ANTI AGE", specialtyData13);
            var subspecialtyData205 = CreateSubspecialtyData("FOTOENVEJECIMIENTO", specialtyData13);
            var subspecialtyData206 = CreateSubspecialtyData("MASCARAS, BLAMQUEADORAS-COLAGENO-ACIDO HIARURONICO", specialtyData13);
            var subspecialtyData207 = CreateSubspecialtyData("HIDROPLASTICAS REJUVENECEDORAS", specialtyData13);
            var subspecialtyData208 = CreateSubspecialtyData("limpieza de cutis con punta de diamante", specialtyData13);
            var subspecialtyData209 = CreateSubspecialtyData("Casimiatria", specialtyData13);
            var subspecialtyData210 = CreateSubspecialtyData("Tratamiento para distintos tipos de pieles", specialtyData13);
            var subspecialtyData211 = CreateSubspecialtyData("Tratamiento para acne y puntos negros", specialtyData13);
            var subspecialtyData212 = CreateSubspecialtyData("Relleno de arrugas", specialtyData13);
            var subspecialtyData213 = CreateSubspecialtyData("Lifting facial sin cirugia", specialtyData13);
            var subspecialtyData214 = CreateSubspecialtyData("Tratamiento con espatula ultrasonica", specialtyData13);
            var subspecialtyData215 = CreateSubspecialtyData("Microdermaabrasion - punta d diamentes", specialtyData13);
            var subspecialtyData292 = CreateSubspecialtyData("Limpieza de cutis", specialtyData13);
            var subspecialtyData216 = CreateSubspecialtyData("Mascarrilla con led", specialtyData13);
            var subspecialtyData217 = CreateSubspecialtyData("Mascara de colageno", specialtyData13);
            var subspecialtyData218 = CreateSubspecialtyData("Drenaje linfatico manual", specialtyData13);
            var subspecialtyData219 = CreateSubspecialtyData("Radiofrecuencia", specialtyData13);
            var subspecialtyData220 = CreateSubspecialtyData("Alta frecuencia", specialtyData13);
            var subspecialtyData221 = CreateSubspecialtyData("Peeling", specialtyData13);
            var subspecialtyData222 = CreateSubspecialtyData("Masoterapia", specialtyData13);
            var subspecialtyData223 = CreateSubspecialtyData("Plasma rico en plaquetas", specialtyData13);
            var subspecialtyData224 = CreateSubspecialtyData("Tratamiento para manchas cutaneas", specialtyData13);
            var subspecialtyData225 = CreateSubspecialtyData("Masaje descontracurante de rostro", specialtyData13);
            var subspecialtyData226 = CreateSubspecialtyData("tratamiento para rosacea", specialtyData13);


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++ Camas Solares
            var subspecialtyData227 = CreateSubspecialtyData("48 Body wave Q Vertical con plataforma vibratoria", specialtyData14);
            var subspecialtyData228 = CreateSubspecialtyData("Ergoline Super Power Classic 300", specialtyData14);
            var subspecialtyData229 = CreateSubspecialtyData("Infinity Ultrasun Q", specialtyData14);


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++ Masajes

            var subspecialtyData230 = CreateSubspecialtyData("Reductores", specialtyData15);
            var subspecialtyData231 = CreateSubspecialtyData("Descontracturantes", specialtyData15);
            var subspecialtyData232 = CreateSubspecialtyData("Relajantes", specialtyData15);
            var subspecialtyData233 = CreateSubspecialtyData("Masaje descontracturante", specialtyData15);
            var subspecialtyData234 = CreateSubspecialtyData("Masajes relajante", specialtyData15);


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++ Depilacion

            var subspecialtyData235 = CreateSubspecialtyData("Sistema español facial y corporal (descartable)", specialtyData16);
            var subspecialtyData236 = CreateSubspecialtyData("Sistema español juveniles", specialtyData16);
            var subspecialtyData237 = CreateSubspecialtyData("Depilacion definitiva", specialtyData16);
            var subspecialtyData238 = CreateSubspecialtyData("Depilación láser", specialtyData16);
            var subspecialtyData239 = CreateSubspecialtyData("Depilación de axilas", specialtyData16);
            var subspecialtyData240 = CreateSubspecialtyData("Depilación de cavado profundo", specialtyData16);
            var subspecialtyData241 = CreateSubspecialtyData("Depilación de gluteos", specialtyData16);
            var subspecialtyData242 = CreateSubspecialtyData("Depilación de media pelvis", specialtyData16);
            var subspecialtyData243 = CreateSubspecialtyData("Depilación de media pierna", specialtyData16);
            var subspecialtyData244 = CreateSubspecialtyData("Depilación de pelvis completa", specialtyData16);
            var subspecialtyData245 = CreateSubspecialtyData("Depilación de pierna entera", specialtyData16);
            var subspecialtyData246 = CreateSubspecialtyData("Depilación de tira de cola", specialtyData16);
            var subspecialtyData247 = CreateSubspecialtyData("Depilación de tira de pelvis", specialtyData16);
            var subspecialtyData248 = CreateSubspecialtyData("Depilación de bozo", specialtyData16);
            var subspecialtyData249 = CreateSubspecialtyData("Depilación hombres espalda", specialtyData16);
            var subspecialtyData250 = CreateSubspecialtyData("Depilación hombres cejas con pinza", specialtyData16);
            var subspecialtyData251 = CreateSubspecialtyData("Depilación hombres cejas con modelación", specialtyData16);
            var subspecialtyData252 = CreateSubspecialtyData("Depilación hombres gluteos", specialtyData16);
            var subspecialtyData253 = CreateSubspecialtyData("Depilación hombres tira de cola", specialtyData16);
            var subspecialtyData254 = CreateSubspecialtyData("Depilacion de cejas", specialtyData16);
            var subspecialtyData255 = CreateSubspecialtyData("Depilación de hombre pierna entera", specialtyData16);
            var subspecialtyData256 = CreateSubspecialtyData("Perfilado de cejas ", specialtyData16);


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++  Pestañas

            var subspecialtyData257 = CreateSubspecialtyData("Extensiones Pelo a pelo", specialtyData17);
            var subspecialtyData258 = CreateSubspecialtyData("Extensiones en grupo", specialtyData17);
            var subspecialtyData259 = CreateSubspecialtyData("Extensiones enteras", specialtyData17);
            var subspecialtyData260 = CreateSubspecialtyData("Permanente y Tintura", specialtyData17);

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++ Spa

            var subspecialtyData261 = CreateSubspecialtyData("Piedas calientes", specialtyData18);
            var subspecialtyData262 = CreateSubspecialtyData("Piedas calientes corporal y facial", specialtyData18);
            var subspecialtyData263 = CreateSubspecialtyData("Manta termica", specialtyData18);
            var subspecialtyData264 = CreateSubspecialtyData("Sauna seco con ezonoterapia", specialtyData18);
            var subspecialtyData265 = CreateSubspecialtyData("Maderoterapia", specialtyData18);
            var subspecialtyData266 = CreateSubspecialtyData("Mascara corporal de chocolate", specialtyData18);
            var subspecialtyData267 = CreateSubspecialtyData("Relajantes", specialtyData18);
            var subspecialtyData268 = CreateSubspecialtyData("Descontracturantes", specialtyData18);
            var subspecialtyData269 = CreateSubspecialtyData("Con Piedas Calientes", specialtyData18);
            var subspecialtyData270 = CreateSubspecialtyData("Con Pindas", specialtyData18);
            var subspecialtyData271 = CreateSubspecialtyData("Reiki", specialtyData18);
            var subspecialtyData272 = CreateSubspecialtyData("Massege Eyes", specialtyData18);
            var subspecialtyData273 = CreateSubspecialtyData("Aromaterapia, Aceites", specialtyData18);
            var subspecialtyData274 = CreateSubspecialtyData("Esenciales Frutales O Herbales", specialtyData18);


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++ Manos Y Pies
            var subspecialtyData275 = CreateSubspecialtyData("Bellea de manos y pies", specialtyData19);
            var subspecialtyData276 = CreateSubspecialtyData("Esmaltado semipermanente en manos y pies", specialtyData19);
            var subspecialtyData277 = CreateSubspecialtyData("Uñas gelificadas o acrilicas en manos", specialtyData19);
            var subspecialtyData278 = CreateSubspecialtyData("Uñas gelificadas o acrilicas en pies", specialtyData19);
            var subspecialtyData279 = CreateSubspecialtyData("Relexologa", specialtyData19);
            var subspecialtyData280 = CreateSubspecialtyData("Spa de pies", specialtyData19);
            var subspecialtyData281 = CreateSubspecialtyData("Parafina en manos y codos", specialtyData19);
            var subspecialtyData282 = CreateSubspecialtyData("Embellecimiento de manos (piel)", specialtyData19);
            var subspecialtyData283 = CreateSubspecialtyData("Nail art", specialtyData19);

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++ Medicina EStetica:
            var subspecialtyData284 = CreateSubspecialtyData("Mesoterapia o IDT", specialtyData20);
            var subspecialtyData285 = CreateSubspecialtyData("Mesoterapia", specialtyData20);
            var subspecialtyData286 = CreateSubspecialtyData("Lipodisolución", specialtyData20);
            var subspecialtyData287 = CreateSubspecialtyData("Mesodysport", specialtyData20);
            var subspecialtyData288 = CreateSubspecialtyData("Plasma Rico en Plaquetas", specialtyData20);
            var subspecialtyData289 = CreateSubspecialtyData("Relleno con Ácido Hialurónico Restylane", specialtyData20);
            var subspecialtyData290 = CreateSubspecialtyData("Toxina Botulínina Dysport", specialtyData20);
            var subspecialtyData291 = CreateSubspecialtyData("Hidratación profunda con Restylane SkinBoosters", specialtyData20);

            Console.Write("OK\n");

            // Creo doctores 
            Console.Write("Doctores\t\t\t");
            var doctor1 = CreateProfessional("Fernando", "Gomez", new List<uint> { 30 }, new List<Hairdressing_Subspecialty> { subespecialityTEST }, HairdressingProfessionalStateEnum.Active,
                new List<Hairdressing_WorkingHours> {
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) }
                }, clinic1);

            var doctor2 = CreateProfessional("Christian", "Russo", new List<uint> { 10, 15 }, new List<Hairdressing_Subspecialty> { subespecialityTEST, subespecialityTEST }, HairdressingProfessionalStateEnum.Active,
                new List<Hairdressing_WorkingHours> {
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) }
                }, clinic1);

            var doctor3 = CreateProfessional("Sabrina", "Fillol", new List<uint> { 10 }, new List<Hairdressing_Subspecialty> { subespecialityTEST }, HairdressingProfessionalStateEnum.Active,
                new List<Hairdressing_WorkingHours> {
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) }
                }, clinic1);

            var doctor4 = CreateProfessional("Pedro", "Perez", new List<uint> { 30 }, new List<Hairdressing_Subspecialty> { subespecialityTEST }, HairdressingProfessionalStateEnum.Active,
                new List<Hairdressing_WorkingHours> {
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) }
                }, clinic2);

            var doctor5 = CreateProfessional("Eduardo", "Martinez", new List<uint> { 10 }, new List<Hairdressing_Subspecialty> { subespecialityTEST }, HairdressingProfessionalStateEnum.Active,
                new List<Hairdressing_WorkingHours> {
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) }
                }, clinic2);

            var doctor6 = CreateProfessional("Eliana", "Lint", new List<uint> { 60 }, new List<Hairdressing_Subspecialty> { subespecialityTEST }, HairdressingProfessionalStateEnum.Active,
                new List<Hairdressing_WorkingHours> {
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) }
                }, clinic3);

            var doctor7 = CreateProfessional("Daniela", "Disig", new List<uint> { 10, 20, 30, 50, 60 }, new List<Hairdressing_Subspecialty> { subespecialityTEST, subespecialityTEST, subespecialityTEST, subespecialityTEST, subespecialityTEST }, HairdressingProfessionalStateEnum.Active,
                new List<Hairdressing_WorkingHours> {
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Hairdressing_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                }, clinic3);
            Console.Write("OK\n");

            /*
            // Creo obras sociales
            Console.Write("Obras sociales\t\t\t");
            var medicalInsurance1 = CreateMedicalInsurance("OSDE", clinic1);
            var medicalInsurance2 = CreateMedicalInsurance("Galeno", clinic1);
            var medicalInsurance3 = CreateMedicalInsurance("Swiss Medical", clinic1);
            var medicalInsurance9 = CreateMedicalInsurance("Medlife", clinic1);

            var medicalInsurance4 = CreateMedicalInsurance("Galeno", clinic2);
            var medicalInsurance5 = CreateMedicalInsurance("Swiss Medical", clinic2);

            var medicalInsurance6 = CreateMedicalInsurance("OSDE", clinic3);
            var medicalInsurance7 = CreateMedicalInsurance("Medlife", clinic3);
            var medicalInsurance8 = CreateMedicalInsurance("Swiss Medical", clinic3);
            Console.Write("OK\n");

            // Creo planes de obras sociales
            Console.Write("Planes de obras sociales\t");
            var medicalPlan1 = CreateMedicalPlan("OSDE 210", medicalInsurance1, clinic1);
            var medicalPlan2 = CreateMedicalPlan("OSDE 310", medicalInsurance1, clinic1);
            var medicalPlan3 = CreateMedicalPlan("OSDE 410", medicalInsurance1, clinic1);
            var medicalPlan4 = CreateMedicalPlan("Galeno XL", medicalInsurance2, clinic1);
            var medicalPlan5 = CreateMedicalPlan("SM 10", medicalInsurance3, clinic1);
            var medicalPlan6 = CreateMedicalPlan("SM 20", medicalInsurance3, clinic1);
            var medicalPlan7 = CreateMedicalPlan("MEDI A", medicalInsurance9, clinic1);
            var medicalPlan8 = CreateMedicalPlan("MEDI B", medicalInsurance9, clinic1);
            var medicalPlan9 = CreateMedicalPlan("MEDI C", medicalInsurance9, clinic1);

            var medicalPlan10 = CreateMedicalPlan("Galeno XL", medicalInsurance4, clinic2);
            var medicalPlan11 = CreateMedicalPlan("SM 10", medicalInsurance5, clinic2);
            var medicalPlan12 = CreateMedicalPlan("SM 20", medicalInsurance5, clinic2);

            var medicalPlan13 = CreateMedicalPlan("OSDE 410", medicalInsurance6, clinic3);
            var medicalPlan14 = CreateMedicalPlan("SM 10", medicalInsurance8, clinic3);
            var medicalPlan15 = CreateMedicalPlan("SM 20", medicalInsurance8, clinic3);
            var medicalPlan16 = CreateMedicalPlan("MEDI A", medicalInsurance7, clinic3);
            var medicalPlan17 = CreateMedicalPlan("MEDI B", medicalInsurance7, clinic3);
            var medicalPlan18 = CreateMedicalPlan("MEDI C", medicalInsurance7, clinic3);
            Console.Write("OK\n");
            */

            // Creo clientes
            /*Console.Write("Clientes\t\t\t");
            var client1 = CreateClientUser("cliente1@asd.com", "cliente1@asd.com", "Pedro", "Gomez", "qwerty 1", "1000001");
            var client2 = CreateClientUser("cliente2@asd.com", "cliente2@asd.com", "Juan", "Martinez", "qwerty 2", "1000002");
            var client3 = CreateClientUser("cliente3@asd.com", "cliente3@asd.com", "Martin", "Violante", "qwerty 3", "1000003");
            var client4 = CreateClientUser("cliente4@asd.com", "cliente4@asd.com", "Leandro", "Lagos", "qwerty 4", "1000004");
            var client5 = CreateClientUser("cliente5@asd.com", "cliente5@asd.com", "Sebastian", "Veliz", "qwerty 5", "1000005");
            var client6 = CreateClientUser("cliente6@asd.com", "cliente6@asd.com", "Maria", "Light", "qwerty 6", "1000006");
            var client7 = CreateClientUser("cliente7@asd.com", "cliente7@asd.com", "Clara", "Florin", "qwerty 7", "1000007");
            var client8 = CreateClientUser("cliente8@asd.com", "cliente8@asd.com", "Daniela", "Brign", "qwerty 8", "1000008");
            var client9 = CreateClientUser("cliente9@asd.com", "cliente9@asd.com", "Sara", "Coronel", "qwerty 9", "1000009");
            var client10 = CreateClientUser("cliente10@asd.com", "cliente10@asd.com", "Matias", "Marquez", "qwerty 10", "1000010");
            var client11 = CreateClientUser("cliente11@asd.com", "cliente11@asd.com", "Daniel", "Perez", "qwerty 11", "1000011");
            var client12 = CreateClientUser("cliente12@asd.com", "cliente12@asd.com", "Javier", "Rito", "qwerty 12", "1000012");
            var client13 = CreateClientUser("cliente13@asd.com", "cliente13@asd.com", "Luis", "Salas", "qwerty 13", "1000013");
            var client14 = CreateClientUser("cliente14@asd.com", "cliente14@asd.com", "Ezequiel", "Morzt", "qwerty 14", "1000014");
            var client15 = CreateClientUser("cliente15@asd.com", "cliente15@asd.com", "Manuela", "Gomez", "qwerty 15", "1000015");
            Console.Write("OK\n");
 */
            // Creo pacientes
            /*Console.Write("Pacientes\t\t\t");
            var patient1 = CreatePatient(medicalPlan1, client1, clinic1);
            var patient2 = CreatePatient(medicalPlan3, client2, clinic1);
            var patient3 = CreatePatient(medicalPlan6, client3, clinic1);
            var patient4 = CreatePatient(medicalPlan7, client4, clinic1);
            var patient5 = CreatePatient(medicalPlan9, client5, clinic1);

            var patient6 = CreatePatient(medicalPlan10, client6, clinic2);
            var patient7 = CreatePatient(medicalPlan10, client7, clinic2);
            var patient8 = CreatePatient(medicalPlan10, client8, clinic2);
            var patient9 = CreatePatient(medicalPlan11, client9, clinic2);
            var patient10 = CreatePatient(medicalPlan11, client10, clinic2);

            var patient11 = CreatePatient(medicalPlan13, client11, clinic3);
            var patient12 = CreatePatient(medicalPlan13, client12, clinic3);
            var patient13 = CreatePatient(medicalPlan15, client13, clinic3);
            var patient14 = CreatePatient(medicalPlan16, client14, clinic3);
            var patient15 = CreatePatient(medicalPlan18, client15, clinic3);
            Console.Write("OK\n"); */

            // Creo turnos
            //Console.Write("Turnos\t\t\t\t");
            /*var appointment1 = CreateAppointment(DateTime.Today.AddDays(-5).AddHours(8).AddMinutes(30), doctor1, subespecialityTEST, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment2 = CreateAppointment(DateTime.Today.AddDays(-3).AddHours(10).AddMinutes(30), doctor1, subespecialityTEST, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment3 = CreateAppointment(DateTime.Today.AddDays(-3).AddHours(12).AddMinutes(0), doctor1, subespecialityTEST, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment4 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(15).AddMinutes(30), doctor1, subespecialityTEST, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment5 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(8).AddMinutes(40), doctor2, subespecialityTEST, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment6 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(8).AddMinutes(50), doctor2, subespecialityTEST, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment7 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(20), doctor2, subespecialityTEST, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment8 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(30), doctor2, subespecialityTEST, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment9 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(50), doctor2, subespecialityTEST, patient4, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment10 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(30), doctor1, subespecialityTEST, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment11 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(14).AddMinutes(20), doctor2, subespecialityTEST, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment12 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), doctor2, subespecialityTEST, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment13 = CreateAppointment(DateTime.Today.AddDays(2).AddHours(16).AddMinutes(0), doctor1, subespecialityTEST, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment14 = CreateAppointment(DateTime.Today.AddDays(3).AddHours(9).AddMinutes(20), doctor2, subespecialityTEST, patient4, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment15 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(16).AddMinutes(40), doctor2, subespecialityTEST, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment16 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(16).AddMinutes(50), doctor2, subespecialityTEST, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment17 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(0), doctor2, subespecialityTEST, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment18 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(10), doctor2, subespecialityTEST, patient4, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment19 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(20), doctor2, subespecialityTEST, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment20 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(40), doctor2, subespecialityTEST, patient1, AppointmentStateEnum.Reserved, null, clinic1);

            var appointment21 = CreateAppointment(DateTime.Today.AddDays(-10).AddHours(8).AddMinutes(0), doctor4, subespecialityTEST, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment22 = CreateAppointment(DateTime.Today.AddDays(-9).AddHours(9).AddMinutes(30), doctor4, subespecialityTEST, patient7, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment23 = CreateAppointment(DateTime.Today.AddDays(-8).AddHours(10).AddMinutes(0), doctor4, subespecialityTEST, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment24 = CreateAppointment(DateTime.Today.AddDays(-7).AddHours(11).AddMinutes(30), doctor4, subespecialityTEST, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment25 = CreateAppointment(DateTime.Today.AddDays(-6).AddHours(12).AddMinutes(0), doctor4, subespecialityTEST, patient10, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment26 = CreateAppointment(DateTime.Today.AddDays(-5).AddHours(13).AddMinutes(30), doctor4, subespecialityTEST, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment27 = CreateAppointment(DateTime.Today.AddDays(-4).AddHours(14).AddMinutes(0), doctor4, subespecialityTEST, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment28 = CreateAppointment(DateTime.Today.AddDays(-3).AddHours(15).AddMinutes(30), doctor4, subespecialityTEST, patient7, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment29 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(16).AddMinutes(0), doctor4, subespecialityTEST, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment30 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(17).AddMinutes(30), doctor4, subespecialityTEST, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment31 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(18).AddMinutes(0), doctor4, subespecialityTEST, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment32 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(19).AddMinutes(30), doctor4, subespecialityTEST, patient10, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment33 = CreateAppointment(DateTime.Today.AddDays(2).AddHours(20).AddMinutes(0), doctor4, subespecialityTEST, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment34 = CreateAppointment(DateTime.Today.AddDays(3).AddHours(9).AddMinutes(30), doctor5, subespecialityTEST, patient7, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment35 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(10).AddMinutes(40), doctor5, subespecialityTEST, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment36 = CreateAppointment(DateTime.Today.AddDays(5).AddHours(11).AddMinutes(50), doctor5, subespecialityTEST, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment37 = CreateAppointment(DateTime.Today.AddDays(6).AddHours(12).AddMinutes(0), doctor5, subespecialityTEST, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment38 = CreateAppointment(DateTime.Today.AddDays(7).AddHours(13).AddMinutes(10), doctor5, subespecialityTEST, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment39 = CreateAppointment(DateTime.Today.AddDays(8).AddHours(14).AddMinutes(20), doctor5, subespecialityTEST, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment40 = CreateAppointment(DateTime.Today.AddDays(9).AddHours(15).AddMinutes(30), doctor5, subespecialityTEST, patient9, AppointmentStateEnum.Reserved, null, clinic2);

            var appointment41 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(10).AddMinutes(30), doctor6, subespecialityTEST, patient15, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment42 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(7).AddMinutes(30), doctor7, subespecialityTEST, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment43 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(16).AddMinutes(30), doctor6, subespecialityTEST, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment44 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(12).AddMinutes(0), doctor7, subespecialityTEST, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment45 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(22).AddMinutes(30), doctor6, subespecialityTEST, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment46 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(8).AddMinutes(0), doctor7, subespecialityTEST, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment47 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(14).AddMinutes(30), doctor6, subespecialityTEST, patient13, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment48 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(9).AddMinutes(0), doctor7, subespecialityTEST, patient3, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment49 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(16).AddMinutes(30), doctor6, subespecialityTEST, patient13, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment50 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(11).AddMinutes(0), doctor7, subespecialityTEST, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment51 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(18).AddMinutes(30), doctor6, subespecialityTEST, patient15, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment52 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(12).AddMinutes(30), doctor7, subespecialityTEST, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment53 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(20).AddMinutes(30), doctor6, subespecialityTEST, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment54 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(30), doctor7, subespecialityTEST, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment55 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(10).AddMinutes(30), doctor6, subespecialityTEST, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment56 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30), doctor7, subespecialityTEST, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment57 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30), doctor6, subespecialityTEST, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment58 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(0), doctor7, subespecialityTEST, patient13, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment59 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(30), doctor6, subespecialityTEST, patient15, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment60 = CreateAppointment(DateTime.Today.AddDays(2).AddHours(11).AddMinutes(30), doctor7, subespecialityTEST, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            Console.Write("OK\n");

            Console.WriteLine("Se finalizo el populado de la base de datos.");
            Console.WriteLine();
             */
        }


        private Hairdressing_Professional CreateProfessional(string firstName, string lastName, List<uint> consultationLengths, List<Hairdressing_Subspecialty> subspecialties, HairdressingProfessionalStateEnum state, List<Hairdressing_WorkingHours> workingHours, Hairdressing hairdressing)
        {
            Hairdressing_Professional professional;

            using (var dbContext = new ApplicationDbContext())
            {
                professional = new Hairdressing_Professional
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = string.Empty,
                    Email = string.Empty,
                    Subspecialties = new List<Hairdressing_ProfessionalSubspecialty>(),
                    State = state,
                    WorkingHours = workingHours,
                    UserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Professionals.Add(professional);
                dbContext.SaveChanges();

                for (var i = 0; i < subspecialties.Count; i++)
                {
                    var doctorSubspecialty = new Hairdressing_ProfessionalSubspecialty
                    {
                        ProfessionalId = professional.Id,
                        SubspecialtyId = subspecialties[i].Id,
                        ConsultationLength = consultationLengths[i]
                    };

                    professional.Subspecialties.Add(doctorSubspecialty);
                }

                dbContext.SaveChanges();
            }

            return professional;
        }


        private Hairdressing_Specialty CreateSpecialty(BusinessType type, string description, Hairdressing hairdressing)
        {
            Hairdressing_Specialty specialty;

            using (var dbContext = new ApplicationDbContext())
            {
                var specialtyData = dbContext.Specialties.FirstOrDefault(s => s.Description == description);

                if (specialtyData == null)
                {
                    specialtyData = new SpecialtyData
                    {
                        Description = description,
                        BusinessTypeId = type.Id
                    };

                    dbContext.Specialties.Add(specialtyData);
                }

                dbContext.SaveChanges();

                specialty = new Hairdressing_Specialty
                {
                    DataId = specialtyData.Id,
                    UserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Specialties.Add(specialty);
                dbContext.SaveChanges();
            }

            return specialty;
        }

        private Hairdressing_Subspecialty CreateSubspecialty(string description, Hairdressing_Specialty specialty, uint consultationLength, Hairdressing hairdressing)
        {
            Hairdressing_Subspecialty subspecialty;

            using (var dbContext = new ApplicationDbContext())
            {
                var subspecialtyData = dbContext.Subspecialties.FirstOrDefault(sp => sp.Description == description);

                if (subspecialtyData == null)
                {
                    subspecialtyData = new SubspecialtyData
                    {
                        Description = description,
                        SpecialtyDataId = specialty.DataId,
                        BusinessTypeId = specialty.Data.BusinessTypeId
                    };

                    dbContext.Subspecialties.Add(subspecialtyData);
                }

                dbContext.SaveChanges();

                subspecialty = new Hairdressing_Subspecialty
                {
                    DataId = subspecialtyData.Id,
                    SpecialtyId = specialty.Id,
                    ConsultationLength = consultationLength,
                    UserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Subspecialties.Add(subspecialty);
                dbContext.SaveChanges();
            }

            return subspecialty;
        }

        private BusinessType CreateBusinessType(string typeName)
        {
            BusinessType type;

            using (var dbContext = new ApplicationDbContext())
            {
                type = new BusinessType
                {
                    Name = typeName
                };

                var a = dbContext.BusinessTypes.Add(type);
                dbContext.SaveChanges();
            }

            return type;
        }

        private void DropDatabaseSchema()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.Database.EnsureDeleted();
            }
        }

        private void CreateDatabaseSchema()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
        }

        private void CreateRoles()
        {
            CreateRole(Roles.Administrator);
            CreateRole(Roles.Employee);
            CreateRole(Roles.Client);
        }

        private void CreateRole(string roleName)
        {
            bool exists = _roleManager.RoleExistsAsync(roleName).Result;

            if (!exists)
            {
                var role = new ApplicationRole
                {
                    Name = roleName
                };

                var result = _roleManager.CreateAsync(role).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }
            }
        }

        private Hairdressing CreateHairdressingUser(BusinessType type, string email, string password, string name, string description, string city, string address, double latitude, double longitude, bool requiresPayment, string clientId, string clientSecret)
        {
            Hairdressing hairdressing;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = _userManager.CreateAsync(user, password).Result;

            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
            }

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Administrator).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var cityData = dbContext.Cities.FirstOrDefault(c => c.Name == city);

                if (cityData == null)
                {
                    cityData = CreateCity(city);
                }

                hairdressing = new Hairdressing
                {
                    Name = name,
                    Description = description,
                    CityId = cityData.Id,
                    Address = address,
                    Latitude = latitude,
                    Longitude = longitude,
                    Logo = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFoAAAAoBAMAAACMbPD7AAAAG1BMVEXMzMyWlpbFxcWjo6OqqqqxsbGcnJy+vr63t7eN+fR5AAAACXBIWXMAAA7EAAAOxAGVKw4bAAAApElEQVQ4je2QsQrCQBBEJ5fLpt2AHxCJWCc2WkZFsTwx9kcQ0ypK6lR+t3eInWw6q3vVLrwdlgECgcAvVFXqy3dm7GvR1ubczMxnjjnZ7ESbclqmJZK6B54c3x6iHYGsslBdByyYMBft2BwZDLxcvuHIXUuoatu6bEwHFGDK5ewUhf8bJ4t7lhUjf9Nw8J2oduWW0U7Sq9ETX2Tvbaxr0Q4E/s8bo1sUV4qjWrAAAAAASUVORK5CYII=",
                    RequiresPayment = requiresPayment,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    UserId = appUser.Id,
                    BusinessTypeId = type.Id
                };

                dbContext.Hairdressings.Add(hairdressing);
                dbContext.SaveChanges();
            }

            return hairdressing;
        }

        private City CreateCity(string cityName)
        {
            City city;

            using (var dbContext = new ApplicationDbContext())
            {
                city = new City
                {
                    Name = cityName
                };

                dbContext.Cities.Add(city);
                dbContext.SaveChanges();
            }

            return city;
        }

        private SpecialtyData CreateSpecialtyData(string description, BusinessType type)
        {
            SpecialtyData specialtyData = new SpecialtyData
            {
                Description = description,
                BusinessTypeId = type.Id
            };

            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.Specialties.Add(specialtyData);
                dbContext.SaveChanges();
            }

            return specialtyData;
        }

        private SubspecialtyData CreateSubspecialtyData(string description, SpecialtyData specialtyData)
        {
            SubspecialtyData subspecialtyData = new SubspecialtyData
            {
                Description = description,
                SpecialtyDataId = specialtyData.Id,
                BusinessTypeId = specialtyData.BusinessTypeId
            };

            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.Subspecialties.Add(subspecialtyData);
                dbContext.SaveChanges();
            }

            return subspecialtyData;
        }

        private SystemClient CreateClientUser(string email, string password, string firstName, string lastName, string address)
        {
            SystemClient client;

            if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = _userManager.CreateAsync(user, password).Result;

            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
            }

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                client = new SystemClient
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    PhoneNumber = "11-1111-1111",
                    UserId = appUser.Id
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();
            }

            return client;
        }

        private Hairdressing_Patient CreatePatient(SystemClient client, Hairdressing hairdressing)
        {
            Hairdressing_Patient patient;

            using (var dbContext = new ApplicationDbContext())
            {
                patient = new Hairdressing_Patient
                {
                    ClientId = client.Id,
                    UserId = hairdressing.Id
                };

                dbContext.Hairdressing_Patients.Add(patient);
                dbContext.SaveChanges();
            }

            return patient;
        }

        private Hairdressing_Appointment CreateAppointment(DateTime dateTime, Hairdressing_Professional professional, Hairdressing_Subspecialty subspecialty, Hairdressing_Patient patient, AppointmentStateEnum state, Hairdressing_Rating rating, Hairdressing hairdressing)
        {
            Hairdressing_Appointment appointment;

            using (var dbContext = new ApplicationDbContext())
            {
                appointment = new Hairdressing_Appointment
                {
                    DateTime = dateTime,
                    ProfessionalId = professional.Id,
                    PatientId = patient.Id,
                    State = state,
                    RatingId = rating?.Id ?? 0,
                    UserId = hairdressing.UserId,
                    SubspecialtyId = subspecialty.Id
                };

                dbContext.Hairdressing_Appointments.Add(appointment);
                dbContext.SaveChanges();
            }

            return appointment;
        }

        private Hairdressing_Employee CreateEmployee(string email, string password, Hairdressing hairdressing)
        {
            Hairdressing_Employee employee;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = _userManager.CreateAsync(user, password).Result;

            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
            }

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Employee).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                employee = new Hairdressing_Employee
                {
                    UserId = appUser.Id,
                    OwnerUserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Employees.Add(employee);
                dbContext.SaveChanges();
            }

            return employee;
        }
    }
}
