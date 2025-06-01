import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura';

const Noir = definePreset(Aura, {
  semantic: {
    primary: {
      50: '{blue.50}',
      100: '{blue.100}',
      200: '{blue.200}',
      300: '{blue.300}',
      400: '{blue.400}',
      500: '{blue.500}',
      600: '{blue.600}',
      700: '{blue.700}',
      800: '{blue.800}',
      900: '{blue.900}',
      950: '{blue.950}'
    },
    colorScheme: {
      dark: {
        primary: {
          color: '{blue.500}',
          inverseColor: '{blue.50}',
          hoverColor: '{blue.400}',
          activeColor: '{blue.300}'
        },
        highlight: {
          background: 'rgba(59,130,246,0.2)', 
          focusBackground: 'rgba(59,130,246,0.3)',
          color: 'rgba(255,255,255,0.87)',
          focusColor: 'rgba(255,255,255,0.87)'
        },
        formField: {
          hoverBorderColor: '{primary.color}'
        },
        surface: {
          ground: '{gray.950}',     // << เปลี่ยนตรงนี้เป็น gray.950
          section: '{gray.950}',    // << 
          card: '{gray.950}',       // <<
          overlay: '{gray.950}',    // << เช่น พวก Dialog พื้นหลัง
          border: '{blue.500}'      // หรือจะปรับ border เป็นฟ้าก็ได้
        }
      }
    }
  },
});

export default Noir;
